using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void HandleBeforePlayerSave(OnServerCharacterSave onSaveBefore)
    {
      DateTime elapsed = DateTime.Now;

      if (!Players.TryGetValue(onSaveBefore.Player.LoginCreature, out Player player))
        return;

      if (player.pcState != Player.PcState.Offline && player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_AUTHORIZED").HasNothing) // On debounce les saves, sauf s'il s'agit d'une déco
      {
        onSaveBefore.PreventSave = true;

        if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_SCHEDULED").HasValue)
        {
          player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_SCHEDULED").Value += 1;
          return;
        }
        else
        {
          player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_SCHEDULED").Value = 1;
          Log.Info($"{player.oid.PlayerName} : scheduling save in 10s");

          player.DebounceSave(player.oid.PlayerName, 1);
          return;
        }
      }
      else
        player.HandlePlayerSave();

      Log.Info($"PC saved in : {(DateTime.Now - elapsed).TotalSeconds} s");
    }
    public partial class Player
    {
      public async void DebounceSave(string playerName, int nbDebounces)
      {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        Task awaitPlayerLeaves = NwTask.WaitUntilValueChanged(() => oid.IsValid, tokenSource.Token);
        Task awaitDebounce = NwTask.WaitUntil(() => !oid.IsValid || oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_SCHEDULED").Value != nbDebounces, tokenSource.Token);
        Task awaitSaveAuthorized = NwTask.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);

        await NwTask.WhenAny(awaitPlayerLeaves, awaitDebounce, awaitSaveAuthorized);
        tokenSource.Cancel();

        if (awaitPlayerLeaves.IsCompletedSuccessfully)
          return;

        if (awaitDebounce.IsCompletedSuccessfully)
        {
          DebounceSave(playerName, nbDebounces + 1);
          return;
        }

        if (awaitSaveAuthorized.IsCompletedSuccessfully)
        {
          if (oid != null)
          {
            oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_SCHEDULED").Delete();
            oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_AUTHORIZED").Value = 1;
            Log.Info($"{oid.PlayerName} : debounce done after {nbDebounces} triggers, save authorized");
            oid.ExportCharacter();
          }
          else
            Log.Info($"{playerName} : already disconnected. Cancelling save.");
        }
      }
      public void HandlePlayerSave()
      {
        FixPolymorphBug();

        // AFK detection
        if (location == oid.LoginCreature.Location)
        {
          isAFK = true;
          Log.Info("Player AFK");
        }
        else if (oid.LoginCreature.Location.Area != null)
          location = oid.LoginCreature.Location;

        if (location.Area?.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value == 0)
        {
          CraftJobProgression();
        }

        dateLastSaved = DateTime.Now;

        SavePlayerCharacterToDatabase();
        SavePlayerStoredMaterialsToDatabase();
        SavePlayerMapPinsToDatabase();
        SavePlayerAreaExplorationStateToDatabase();
        SavePlayerChatColorsToDatabase();
        HandleExpiredContracts();
        HandleExpiredBuyOrders();
        HandleExpiredSellOrders();
        HandleNewMails();

        oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_SCHEDULED").Delete();
        oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_AUTHORIZED").Delete();

        Log.Info("Finished saving player");
      }
      private void FixPolymorphBug()
      {
        /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à parcourir tous ses buffs et à les réappliquer dans l'event AFTER de la sauvegarde*/

        if (oid.LoginCreature.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
        {
          List<Effect> effectList = oid.LoginCreature.ActiveEffects.Where(e => e.EffectType == EffectType.Polymorph).ToList();

          foreach (Effect eff in effectList)
            oid.LoginCreature.RemoveEffect(eff);

          Log.Info($"Polymorph detected, saving effect list");

          RestorePolymorph(effectList);
        }
      }
      private async void RestorePolymorph(List<Effect> effectList)
      {
        /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
         * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
         * sont réappliquées. 
         * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
         * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
         * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
         * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
         * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/

        if (oid == null)
          return;

        await NwTask.Delay(TimeSpan.FromSeconds(0.1));

        Log.Info($"Polymorph detected, restoring effect list on {oid.LoginCreature.Name}");

        foreach (Effect eff in effectList)
          oid.LoginCreature.ApplyEffect(eff.DurationType, eff, TimeSpan.FromSeconds((double)eff.DurationRemaining));
      }
      private async void SavePlayerCharacterToDatabase()
      {
        string areaTag;
        string position;
        string facing;

        if (location.Area != null)
        {
          areaTag = location.Area.Tag;
          position = location.Position.ToString();
          facing = location.Rotation.ToString();
        }
        else
        {
          areaTag = previousLocation.Area.Tag;
          position = previousLocation.Position.ToString();
          facing = previousLocation.Rotation.ToString();
        }

        string firstName = oid.LoginCreature.OriginalFirstName;
        string lastName = oid.LoginCreature.OriginalLastName;
        string health = oid.LoginCreature.HP.ToString();

        await NwTask.Delay(TimeSpan.FromSeconds(0.2));

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "characterName", $"{firstName} {lastName}" },
          new string[] { "areaTag", areaTag }, new string[] { "position", position }, new string[] { "facing", facing }, new string[] { "currentHP", health }, new string[] { "bankGold", bankGold.ToString() },
          new string[] { "dateLastSaved", dateLastSaved.ToString() }, new string[] { "previousSPCalculation", previousSPCalculation.ToString() },
          new string[] { "currentCraftJob", craftJob.baseItemType.ToString() }, new string[] { "currentCraftObject", craftJob.craftedItem }, new string[] { "currentCraftJobRemainingTime", craftJob.remainingTime.ToString() },
          new string[] { "currentCraftJobMaterial", craftJob.material }, new string[] { "currentCraftJobMaterial", craftJob.material }, new string[] { "pveArenaCurrentPoints", pveArena.currentPoints.ToString() },
          new string[] { "menuOriginTop", menu.originTop.ToString() }, new string[] { "menuOriginLeft", menu.originLeft.ToString() },
          new string[] { "alchemyCauldron", JsonConvert.SerializeObject(alchemyCauldron) }, new string[] { "serializedLearnables", JsonConvert.SerializeObject(learnables) } },
          new List<string[]>() { new string[] { "rowid", characterId.ToString() } });
      }
      private void SavePlayerStoredMaterialsToDatabase()
      {
        if (materialStock.Count > 0)
        {
          foreach (string material in materialStock.Keys)
          {
            SqLiteUtils.InsertQuery("playerMaterialStorage",
            new List<string[]>() { new string[] { "characterId", characterId.ToString() },
            new string[] { "materialName", material },
            new string[] { "materialStock", materialStock[material].ToString() } },
              new List<string>() { "characterId", "materialName" },
              new List<string[]>() { new string[] { "materialStock" } },
              new List<string>() { { "characterId" }, { "materialName" } });
          }
        }
      }
      private void SavePlayerMapPinsToDatabase()
      {
        if (mapPinDictionnary.Count > 0)
        {
          foreach (MapPin mapPin in mapPinDictionnary.Values)
          {
            SqLiteUtils.InsertQuery("playerMapPins",
            new List<string[]>() {
            new string[] { "characterId", characterId.ToString() },
            new string[] { "mapPinId", mapPin.id.ToString()},
            new string[] { "areaTag", mapPin.areaTag },
            new string[] { "x", mapPin.x.ToString() },
            new string[] { "y", mapPin.y.ToString() },
            new string[] { "note", mapPin.note } },
            new List<string>() { "characterId", "mapPinId" },
            new List<string[]>() { new string[] { "x" }, new string[] { "y" }, new string[] { "note" } });
          }
        }
      }
      private void SavePlayerAreaExplorationStateToDatabase()
      {
        if (areaExplorationStateDictionnary.Count > 0)
        {
          foreach (KeyValuePair<string, byte[]> explorationStateListEntry in areaExplorationStateDictionnary)
          {
            SqLiteUtils.InsertQuery("playerAreaExplorationState",
            new List<string[]>() {
            new string[] { "characterId", characterId.ToString() },
            new string[] { "areaTag", explorationStateListEntry.Key},
            new string[] { "explorationState", explorationStateListEntry.Value.ToBase64EncodedString() } },
            new List<string>() { "characterId", "areaTag" },
            new List<string[]>() { new string[] { "explorationState" } });
          }
        }
      }
      private void SavePlayerChatColorsToDatabase()
      {
        if (chatColors.Count > 0)
        {
          foreach (KeyValuePair<ChatChannel, Color> chatColorEntry in chatColors)
          {
            SqLiteUtils.InsertQuery("chatColors",
            new List<string[]>() {
            new string[] { "accountId", accountId.ToString() },
            new string[] { "channel", ((int)chatColorEntry.Key).ToString()},
            new string[] { "color", chatColorEntry.Value.ToInt().ToString() } },
            new List<string>() { "accountId", "channel" },
            new List<string[]>() { new string[] { "color" } });
          }
        }
      }
      private void HandleExpiredContracts()
      {
        var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
              new List<string>() { { "expirationDate" }, { "rowid" } },
              new List<string[]>() { new string[] { "characterId", characterId.ToString() } });

        foreach (var contract in result.Results)
        {
          int contractId = contract.GetInt(1);

          if ((DateTime.Parse(contract.GetString(0)) - DateTime.Now).TotalSeconds < 0)
          {
            Task contractExpiration = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));
              DeleteExpiredContract(contractId);
            });
          }
        }
      }
      private void DeleteExpiredContract(int contractId)
      {
        var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
            new List<string>() { { "serializedContract" } },
            new List<string[]>() { new string[] { "ROWID", contractId.ToString() } });

        if (result.Result != null)
        {
          foreach (string materialString in result.Result.GetString(0).Split("|"))
          {
            string[] descriptionString = materialString.Split("$");
            if (descriptionString.Length == 3)
            {
              if (materialStock.ContainsKey(descriptionString[0]))
                materialStock[descriptionString[0]] += Int32.Parse(descriptionString[1]);
              else
                materialStock.Add(descriptionString[0], Int32.Parse(descriptionString[1]));

              oid.SendServerMessage($"Expiration du contrat {contractId} - {descriptionString[1]} unité(s) de {descriptionString[0]} ont été réintégrées à votre entrepôt.");
            }
          }

          SqLiteUtils.DeletionQuery("playerPrivateContracts",
            new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
        }
      }
      private void HandleExpiredBuyOrders()
      {
        var result = SqLiteUtils.SelectQuery("playerBuyOrders",
            new List<string>() { { "expirationDate" }, { "rowid" } },
            new List<string[]>() { new string[] { "characterId", characterId.ToString() } });

        foreach (var contract in result.Results)
        {
          int contractId = contract.GetInt(1);

          if ((DateTime.Parse(contract.GetString(0)) - DateTime.Now).TotalSeconds < 0)
          {
            Task contractExpiration = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));
              DeleteExpiredBuyOrder(contractId);
            });
          }
        }
      }
      private void DeleteExpiredBuyOrder(int contractId)
      {
        var result = SqLiteUtils.SelectQuery("playerBuyOrders",
            new List<string>() { { "quantity" }, { "unitPrice" } },
            new List<string[]>() { new string[] { "rowid", characterId.ToString() } });

        if (result.Result != null)
        {
          int gold = result.Result.GetInt(0) * result.Result.GetInt(1);
          bankGold += gold;
          oid.SendServerMessage($"Expiration de l'ordre d'achat {contractId} - {gold} pièce(s) d'or ont été reversées à votre banque.");

          SqLiteUtils.DeletionQuery("playerBuyOrders",
              new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
        }
      }
      private void HandleExpiredSellOrders()
      {
        var result = SqLiteUtils.SelectQuery("playerSellOrders",
            new List<string>() { { "expirationDate" }, { "rowid" } },
            new List<string[]>() { new string[] { "characterId", characterId.ToString() } });

        foreach (var sellOrder in result.Results)
        {
          int contractId = sellOrder.GetInt(1);

          if ((DateTime.Parse(sellOrder.GetString(0)) - DateTime.Now).TotalSeconds < 0)
          {
            Task contractExpiration = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));
              DeleteExpiredSellOrder(contractId);
            });
          }
        }
      }
      private void DeleteExpiredSellOrder(int contractId)
      {
        var result = SqLiteUtils.SelectQuery("playerSellOrders",
            new List<string>() { { "playerSellOrders" }, { "quantity" } },
            new List<string[]>() { new string[] { "rowid", contractId.ToString() } });

        if (result.Result != null)
        {
          string material = result.Result.GetString(0);
          int quantity = result.Result.GetInt(1);

          if (materialStock.ContainsKey(material))
            materialStock[material] += quantity;
          else
            materialStock.Add(material, quantity);

          oid.SendServerMessage($"Expiration de l'ordre de vente {contractId} - {quantity} unité(s) de {material} sont en cours de transfert vers votre entrepôt.");

          SqLiteUtils.DeletionQuery("playerSellOrders",
             new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
        }
      }
      private void HandleNewMails()
      {
        var result = SqLiteUtils.SelectQuery("messenger",
            new List<string>() { { "count (*)" } },
            new List<string[]>() { new string[] { "characterId", characterId.ToString() }, new string[] { "read", "0" } });

        if (result.Result != null)
        {
          int nbMails = result.Result.GetInt(0);
          if (nbMails > 0)
            oid.SendServerMessage($"{nbMails.ToString().ColorString(ColorConstants.White)} lettres non lues se trouvent dans votre boîte aux lettres.", ColorConstants.Pink);
        }
      }
    }
  }
}
