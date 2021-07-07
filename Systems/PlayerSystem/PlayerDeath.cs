using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void HandlePlayerDeath(ModuleEvents.OnPlayerDeath onPlayerDeath)
    {
      if (Players.TryGetValue(onPlayerDeath.DeadPlayer.LoginCreature, out Player player))
      {
        onPlayerDeath.DeadPlayer.SendServerMessage("Tout se brouille autour de vous. Avant de perdre connaissance, vous sentez comme un étrange maëlstrom vous aspirer.");

        API.Location playerDeathLocation = player.oid.LoginCreature.Location;
        
        Task handleDeath = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(3));
          player.oid.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>("WP__RESPAWN_AREA").FirstOrDefault().Location;
          await NwTask.WaitUntil(() => player.oid.LoginCreature.Area != null);
          SendPlayerToLimbo(player);
          CreatePlayerCorpse(player, playerDeathLocation);
          player.oid.SendServerMessage($"{player.oid.LoginCreature.Gold.ToString().ColorString(ColorConstants.White)} pièces d'or ont été abandonnées sur place !", ColorConstants.Red);
          player.oid.LoginCreature.Gold = 0;
          StripPlayerOfCraftResources(player);
        });
      }
    }
    private static void CreatePlayerCorpse(Player player, Location deathLocation)
    {
      NwCreature oPCCorpse = player.oid.LoginCreature.Clone(deathLocation, "pccorpse");

      foreach (NwItem item in oPCCorpse.Inventory.Items)
        item.Destroy();

      NwItem oCorpseItem = NwItem.Create("item_pccorpse", oPCCorpse.Location);
      oPCCorpse.AcquireItem(oCorpseItem);
      
      oPCCorpse.Lootable = true;
      oPCCorpse.Name = $"Corps inconscient de {player.oid.LoginCreature.Name}";
      oPCCorpse.Description = $"Corps inconscient de {player.oid.LoginCreature.Name}. \n\n\n Allez savoir combien de temps il va tenir dans cet état.";
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, oPCCorpse, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
      
      oPCCorpse.GetLocalVariable<int>("_PC_ID").Value = player.characterId;
      
      for (int i = 0; i <= (int)InventorySlot.Bolts; i++)
        if(oPCCorpse.GetItemInSlot((InventorySlot)i) != null)
          oPCCorpse.GetItemInSlot((InventorySlot)i).Droppable = false;
      
      oCorpseItem.GetLocalVariable<int>("_PC_ID").Value = player.characterId;
      oCorpseItem.Name = $"Corps inconscient de {player.oid.LoginCreature.Name}";
      oCorpseItem.Description = $"Corps inconscient de {player.oid.LoginCreature.Name}\n\n\n Pas très ragoûtant. Allez savoir combien de temps il va tenir avant de se lâcher.";
      oCorpseItem.Droppable = true;

      oCorpseItem.GetLocalVariable<string>("_SERIALIZED_CORPSE").Value = oPCCorpse.Serialize().ToBase64EncodedString();
      player.deathCorpse = oPCCorpse;
      
      SavePlayerCorpseToDatabase(player.characterId, player.deathCorpse);

      SetupPCCorpse(oPCCorpse);

      Log.Info($"Corpse {oPCCorpse.Name} created");
    }
    private static void StripPlayerOfCraftResources(Player player)
    {
      Log.Info($"{player.oid.LoginCreature.Name} dead. Stripping him of craft resources");
      foreach (NwItem oItem in player.oid.LoginCreature.Inventory.Items.Where(i => Craft.Collect.System.IsItemCraftMaterial(i.Tag) || i.Tag == "blueprint"))
      {
        Log.Info($"{oItem.Name} stripped");
        oItem.Clone(player.deathCorpse).Droppable = true;
        oItem.Destroy();
      }
    }
    public static void SavePlayerCorpseToDatabase(int characterId, NwCreature deathCorpse)
    {
      SqLiteUtils.InsertQuery("playerDeathCorpses",
          new List<string[]>() { new string[] { "characterId", characterId.ToString() }, new string[] { "deathCorpse", deathCorpse.Serialize().ToBase64EncodedString() }, new string[] { "areaTag", deathCorpse.Area.Tag }, new string[] { "position", deathCorpse.Position.ToString() } });
    }
    private static void SendPlayerToLimbo(Player player)
    {
      player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRestorationGreater));
      player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Resurrection());
      player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Heal(player.oid.LoginCreature.MaxHP));
    }
    public static void Respawn(Player player)
    {
      DestroyPlayerCorpse(player);
      player.oid.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>("WP_RESPAWN_DISPENSAIRE").FirstOrDefault()?.Location;

      if (player.oid.LoginCreature.GetItemInSlot(InventorySlot.Neck)?.Tag != "amulettorillink")
      {
        Effect eff = Effect.SpellFailure(50);
        eff.Tag = "erylies_spell_failure";
        eff.SubType = EffectSubType.Supernatural;
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, eff);
      }

      player.bankGold -= 50;
      player.oid.SendServerMessage("Afin de vous remettre sur pied, les 'soigneurs' ont demandé à la banque de prélever 50 pièces d'or sur votre compte.");

      //NWScript.SendMessageToPC(player.oid, "Votre récente déconvenue vous a affligé d'une blessure durable. Il va falloir passer du temps en rééducation pour vous en débarrasser");

      // TODO : refaire le système de malus en utilisant le plugin feat de nwnx
      /*int iRandomMalus = Utils.random.Next(1130, 1130); // TODO : il faudra mettre en paramètre de conf le range des feat ID pour les malus

      if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, iRandomMalus) != (int)Feat.Invalid)
      {
        int successorId;
        if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", iRandomMalus), out successorId))
        {
          CreaturePlugin.AddFeat(player.oid, successorId);
          iRandomMalus = successorId;
        }
      }
      else
        CreaturePlugin.AddFeat(player.oid, iRandomMalus);

      Func<Player, int, int> handler;
      if (SkillSystem.RegisterAddCustomFeatEffect.TryGetValue(iRandomMalus, out handler))
      {
        try
        {
          handler.Invoke(player, iRandomMalus);
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }*/
    }
    public static void DestroyPlayerCorpse(Player player)
    {
      DeletePlayerCorpseFromDatabase(player.characterId);

      NwCreature oCorpse = NwObject.FindObjectsWithTag<NwCreature>("pccorpse").Where(c => c.GetLocalVariable<int>("_PC_ID").Value == player.characterId).FirstOrDefault();
      oCorpse?.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(o => o.Tag == "pccorpse_bodybag")?.Destroy();
      oCorpse?.Destroy();
      NwObject.FindObjectsWithTag<NwItem>("item_pccorpse").FirstOrDefault(c => c.GetLocalVariable<int>("_PC_ID").Value == player.characterId)?.Destroy();
    }
    public static void DeletePlayerCorpseFromDatabase(int characterId)
    {
      SqLiteUtils.DeletionQuery("playerDeathCorpses",
          new Dictionary<string, string>() { { "characterId", characterId.ToString() } });
    }
    public static void SetupPCCorpse(NwCreature oPCCorpse)
    {
      Task settingUpCorpse = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        oPCCorpse.ApplyEffect(EffectDuration.Instant, API.Effect.Death());
        NwWaypoint wp = NwWaypoint.Create("NW_WAYPOINT001", oPCCorpse.Location, false, $"wp_pccorpse_{oPCCorpse.GetLocalVariable<int>("_PC_ID").Value}");
        await NwTask.Delay(TimeSpan.FromSeconds(0.8));
        VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, oPCCorpse, VisibilityPlugin.NWNX_VISIBILITY_DEFAULT);
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        oPCCorpse.Tag = "pccorpse";
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        wp.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(b => b.Tag == "BodyBag").Tag = "pccorpse_bodybag";
        wp.Destroy(0.1f);
      });
    }
  }
}
