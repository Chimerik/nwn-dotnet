using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void HandleBeforePlayerSave(OnServerCharacterSave onSaveBefore)
    {
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
          LogUtils.LogMessage($"{player.oid.PlayerName} : scheduling save in 10s", LogUtils.LogType.PlayerSaveSystem);

          player.DebounceSave(player.oid.PlayerName, 1);
          return;
        }
      }
      else
        player.HandlePlayerSave();
    }
    public partial class Player
    {
      public async void DebounceSave(string playerName, int nbDebounces)
      {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        Task awaitPlayerLeaves = NwTask.WaitUntil(() => pcState == PcState.Offline || oid == null || !oid.IsValid, tokenSource.Token);
        Task awaitDebounce = NwTask.WaitUntil(() => oid != null && oid.IsValid && oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_SAVE_SCHEDULED").Value != nbDebounces, tokenSource.Token);
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
            LogUtils.LogMessage($"{oid.PlayerName} : debounce done after {nbDebounces} triggers, save authorized", LogUtils.LogType.PlayerSaveSystem);
            oid.ExportCharacter();
          }
          else
            LogUtils.LogMessage($"{playerName} : already disconnected. Cancelling save.", LogUtils.LogType.PlayerSaveSystem);
        }
      }
      public void HandlePlayerSave()
      {
        FixPolymorphBug();

        if (oid.LoginCreature.Location.Area is not null)
          location = oid.LoginCreature.Location;

        if (windowRectangles == null)
        {
          Utils.LogMessageToDMs($"ATTENTION - {oid.LoginCreature.Name} n'a pas été correctement initialisé et n'a pas pu être sauvegardé !");
          return;
        }

        SavePlayerAccountToDatabase();
        SavePlayerCharacterToDatabase();
        HandleExpiredContracts();

        oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_SCHEDULED").Delete();
        oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SAVE_AUTHORIZED").Delete();
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

          LogUtils.LogMessage($"Polymorph detected, saving effect list", LogUtils.LogType.PlayerSaveSystem);

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

        if (oid == null || oid.ControlledCreature == null)
          return;

        await NwTask.Delay(TimeSpan.FromSeconds(0.1));

        LogUtils.LogMessage($"Polymorph detected, restoring effect list on {oid.LoginCreature.Name}", LogUtils.LogType.PlayerSaveSystem);

        foreach (Effect eff in effectList)
          oid.LoginCreature.ApplyEffect(eff.DurationType, eff, TimeSpan.FromSeconds((double)eff.DurationRemaining));
      }
      private async void SavePlayerAccountToDatabase()
      {
        if (windowRectangles.Count < 1)
          return;

        Task<string> serializeWindowRectangles = Task.Run(() => JsonConvert.SerializeObject(windowRectangles));
        Task<string> serializeMutedPlayers = Task.Run(() => JsonConvert.SerializeObject(mutedList));
        Task<string> serializeChatColors = Task.Run(() => JsonConvert.SerializeObject(chatColors));
        Task<string> serializeCoolodownPosition = Task.Run(() => JsonConvert.SerializeObject(cooldownPositions));

        await Task.WhenAll(serializeWindowRectangles, serializeMutedPlayers, serializeChatColors, serializeCoolodownPosition);

        SqLiteUtils.UpdateQuery("PlayerAccounts",
          new List<string[]>() { new string[] { "windowRectangles", serializeWindowRectangles.Result }, new string[] { "mutedPlayers", serializeMutedPlayers.Result },
            new string[] { "bonusRolePlay", bonusRolePlay.ToString() }, new string[] { "chatColors", serializeChatColors.Result }, 
            new string[] { "hideFromPlayerList", hideFromPlayerList.ToInt().ToString() }, new string[] { "cooldownPosition", serializeCoolodownPosition.Result  } },
          new List<string[]>() { new string[] { "rowid", accountId.ToString() } });
      }
      private async void SavePlayerCharacterToDatabase()
      {
        DateTime elapsed = DateTime.Now;

        string serializedLocation = location.Area is not null ? SqLiteUtils.SerializeLocation(location) : SqLiteUtils.SerializeLocation(previousLocation);
        string firstName = oid.LoginCreature.OriginalFirstName;
        string lastName = oid.LoginCreature.OriginalLastName;
        string health = oid.LoginCreature.HP.ToString();

        await NwTask.Delay(TimeSpan.FromSeconds(0.2));

        Task<string> serializeAlchemyCauldron = Task.Run(() => JsonConvert.SerializeObject(alchemyCauldron));
        Task<string> serializeExplorationState = Task.Run(() => JsonConvert.SerializeObject(areaExplorationStateDictionnary));
        Task<string> serializeGrimoires = Task.Run(() => JsonConvert.SerializeObject(grimoires));
        Task<string> serializeQuickbars = Task.Run(() => JsonConvert.SerializeObject(quickbars));
        Task<string> serializeItemAppearances = Task.Run(() => JsonConvert.SerializeObject(itemAppearances));
        Task<string> serializeDescriptions = Task.Run(() => JsonConvert.SerializeObject(descriptions));
        Task<string> serializeEndurance = Task.Run(() => JsonConvert.SerializeObject(endurance));

        if (activeLearnable != null)
          activeLearnable.spLastCalculation = DateTime.Now;

        Task<string> serializeLearnableSkills = Task.Run(() =>
        {
          Dictionary<int, LearnableSkill.SerializableLearnableSkill> serializableSkills = new Dictionary<int, LearnableSkill.SerializableLearnableSkill>();
          foreach (var kvp in learnableSkills)
            serializableSkills.Add(kvp.Key, new LearnableSkill.SerializableLearnableSkill(kvp.Value));

          return JsonConvert.SerializeObject(serializableSkills);
        });

        Task<string> serializeLearnableSpells = Task.Run(() =>
        {
          Dictionary<int, LearnableSpell.SerializableLearnableSpell> serializableSpells = new Dictionary<int, LearnableSpell.SerializableLearnableSpell>();
          foreach (var kvp in learnableSpells)
            serializableSpells.Add(kvp.Key, new LearnableSpell.SerializableLearnableSpell(kvp.Value));

          return JsonConvert.SerializeObject(serializableSpells);
        });

        Task<string> serializeJob = Task.Run(() => craftJob != null ? JsonConvert.SerializeObject(new CraftJob.SerializableCraftJob(craftJob)) : JsonConvert.SerializeObject(craftJob));

        Task<string> serializeCraftResource = Task.Run(() =>
        {
          List<CraftResource.SerializableCraftResource> serializableCraftResources = new List<CraftResource.SerializableCraftResource>();
          foreach (CraftResource res in craftResourceStock)
            serializableCraftResources.Add(new CraftResource.SerializableCraftResource(res));

          return JsonConvert.SerializeObject(serializableCraftResources);
        });

        Task<string> serializeMails = Task.Run(() =>
        {
          List<Mail.SerializableMail> serializableMails = new();
          foreach (var mail in mails)
            if(!mail.expirationDate.HasValue || mail.expirationDate.Value > DateTime.Now)
              serializableMails.Add(new Mail.SerializableMail(mail));

          return JsonConvert.SerializeObject(serializableMails);
        });

        Task<string> serializeSubscriptions = Task.Run(() =>
        {
          List<Subscription.SerializableSubscription> serializableSubscriptions = new();
          foreach (var subscription in subscriptions)
              serializableSubscriptions.Add(new Subscription.SerializableSubscription(subscription));

          return JsonConvert.SerializeObject(serializableSubscriptions);
        });

        await Task.WhenAll(serializeAlchemyCauldron, serializeLearnableSkills, serializeLearnableSpells, serializeExplorationState, serializeJob, serializeCraftResource, serializeGrimoires, serializeQuickbars, serializeItemAppearances, serializeDescriptions, serializeMails, serializeSubscriptions, serializeEndurance);

        SqLiteUtils.UpdateQuery("playerCharacters",
        new List<string[]>() { new string[] { "characterName", $"{firstName} {lastName}" },
          new string[] { "location", serializedLocation }, new string[] { "currentHP", health }, new string[] { "bankGold", bankGold.ToString() },
          new string[] { "pveArenaCurrentPoints", pveArena.currentPoints.ToString() }, new string[] { "menuOriginTop", menu.originTop.ToString() },
          new string[] { "menuOriginLeft", menu.originLeft.ToString() }, new string[] { "alchemyCauldron", serializeAlchemyCauldron.Result },
          new string[] { "serializedLearnableSkills", serializeLearnableSkills.Result }, new string[] { "serializedLearnableSpells", serializeLearnableSpells.Result },
          new string[] { "explorationState", serializeExplorationState.Result }, new string[] { "quickbars", serializeQuickbars.Result },
          new string[] { "itemAppearances", serializeItemAppearances.Result }, new string[] { "descriptions", serializeDescriptions.Result },
          new string[] { "craftJob", serializeJob.Result  }, new string[] { "materialStorage", serializeCraftResource.Result }, new string[] { "grimoires", serializeGrimoires.Result },
          new string[] { "mails", serializeMails.Result  }, new string[] { "subscriptions", serializeSubscriptions.Result  }, new string[] { "endurance", serializeEndurance.Result } },
        new List<string[]>() { new string[] { "rowid", characterId.ToString() } });

        LogUtils.LogMessage($"ASYNC SAVE FINALIZED for {firstName} {lastName} in {Math.Round((DateTime.Now - elapsed).TotalSeconds, 3, MidpointRounding.ToEven)} s", LogUtils.LogType.PlayerSaveSystem);

        if (pcState == PcState.Offline)
          oid = null;
      }
      private async void HandleExpiredContracts()
      {
        var result = await SqLiteUtils.SelectQueryAsync("playerPrivateContracts",
              new List<string>() { { "expirationDate" }, { "rowid" } },
              new List<string[]>() { new string[] { "characterId", characterId.ToString() } });

        if (result != null)
          foreach (var contract in result)
          {
            int contractId = int.Parse(contract[1]);

            if ((DateTime.Parse(contract[0]) - DateTime.Now).TotalSeconds < 0)
            {
              Task contractExpiration = NwTask.Run(async () =>
              {
                await NwTask.Delay(TimeSpan.FromSeconds(0.2));
                DeleteExpiredContract(contractId);
              });
            }
          }
      }
      private async void DeleteExpiredContract(int contractId)
      {
        var result = await SqLiteUtils.SelectQueryAsync("playerPrivateContracts",
            new List<string>() { { "serializedContract" } },
            new List<string[]>() { new string[] { "ROWID", contractId.ToString() } });

        await NwTask.SwitchToMainThread();

        if (result != null)
        {
          foreach (string materialString in result[0][0].Split("|"))
          {
            string[] descriptionString = materialString.Split("$");
            if (descriptionString.Length == 3)
            {
              /*if (materialStock.ContainsKey(descriptionString[0]))
                materialStock[descriptionString[0]] += Int32.Parse(descriptionString[1]);
              else
                materialStock.Add(descriptionString[0], Int32.Parse(descriptionString[1]));*/

              oid.SendServerMessage($"Expiration du contrat {contractId} - {descriptionString[1]} unité(s) de {descriptionString[0]} ont été réintégrées à votre entrepôt.");
            }
          }

          SqLiteUtils.DeletionQuery("playerPrivateContracts",
            new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
        }
      }
    }
  }
}
