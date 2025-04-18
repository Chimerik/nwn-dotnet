﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    /*public static async void OnDeathSoulReap(ModuleEvents.OnPlayerDeath onPlayerDeath)
    {
      foreach (NwPlayer player in NwModule.Instance.Players)
        if (player?.ControlledCreature?.Area == onPlayerDeath.DeadPlayer.LoginCreature?.Area 
          && player?.ControlledCreature.DistanceSquared(onPlayerDeath.DeadPlayer.LoginCreature) < 600
          && Players.TryGetValue(player.LoginCreature, out Player reaper) && reaper.GetAttributeLevel(SkillSystem.Attribut.SoulReaping) > 0
          && reaper.endurance.regenerableMana > 0
          && reaper.soulReapTriggers < (player.LoginCreature.GetAbilityScore(Ability.Intelligence, true) - 10) / 4)
        {
          int reaperLevel = reaper.GetAttributeLevel(SkillSystem.Attribut.SoulReaping);
          reaper.endurance.currentMana += reaperLevel;
          reaper.endurance.regenerableMana -= reaperLevel;

          reaper.soulReapTriggers += 1;

          await NwTask.Delay(TimeSpan.FromSeconds(15));
          reaper.soulReapTriggers -= 1;
        }
    }*/

    public static void HandlePlayerDying(ModuleEvents.OnPlayerDying onPlayerDying)
    {
      //ModuleSystem.Log.Info($"OnPlayerDying Triggered - HP : {onPlayerDying.Player.LoginCreature.HP}");

      var creature = onPlayerDying.Player.LoginCreature;

      /*if (creature.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
      {
        EffectUtils.RemoveEffectType(creature, EffectType.Polymorph);
      }
      else if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionContreLaMortEffectTag))
      {
        creature.HP = 1;

        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.ProtectionContreLaMortEffectTag);
        StringUtils.DisplayStringToAllPlayersNearTarget(creature, $"{creature.Name.ColorString(ColorConstants.Cyan)} - Protection contre la Mort", StringUtils.gold, true, true);
      }
      else if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.EnduranceImplacableEffectTag))
      {
        creature.HP = 1;

        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.EnduranceImplacableEffectTag);
        creature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Delete();
        StringUtils.DisplayStringToAllPlayersNearTarget(creature, $"{creature.Name.ColorString(ColorConstants.Cyan)} - Endurance Implacable", StringUtils.gold, true, true);

        if (creature.KnowsFeat((Feat)CustomSkill.FureurOrc)
          && creature.CurrentAction == Anvil.API.Action.AttackObject)
        {
          var reaction = creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

          if (reaction is not null)
          {
            creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.FureurOrcBonusAttackVariable).Value = 1;
            creature.RemoveEffect(reaction);
          }
        }
      }
      else if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.SentinelleImmortelleEffectTag))
      {
        creature.HP = 1;

        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.SentinelleImmortelleEffectTag);
        creature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Delete();
        StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Sentinelle Immortelle", StringUtils.gold, true);
      }
      else*/ if(creature.HP < 1)
      {
        creature.ApplyEffect(EffectDuration.Instant, Effect.Death(false, false));
      }
    }

    public static void HandlePlayerDeath(ModuleEvents.OnPlayerDeath onPlayerDeath)
    {
      //ModuleSystem.Log.Info($"OnPlayerDeath Triggered - HP : {onPlayerDeath.DeadPlayer.LoginCreature.HP}");

      if (Players.TryGetValue(onPlayerDeath.DeadPlayer.LoginCreature, out Player player))
      {
        onPlayerDeath.DeadPlayer.SendServerMessage("Tout se brouille autour de vous. Avant de perdre connaissance, vous sentez comme un étrange maëlstrom vous aspirer.");

        onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Resurrection());
        onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Heal(onPlayerDeath.DeadPlayer.LoginCreature.MaxHP));

        /*Location playerDeathLocation = player.oid.LoginCreature.Location;
        
        Task handleDeath = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(3));
          player.oid.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>("WP__RESPAWN_AREA").FirstOrDefault().Location;
          await NwTask.WaitUntil(() => player.oid.LoginCreature.Area != null);
          player.SendPlayerToLimbo();
          player.CreatePlayerCorpse(playerDeathLocation);
          player.oid.SendServerMessage($"{player.oid.LoginCreature.Gold.ToString().ColorString(ColorConstants.White)} pièces d'or ont été abandonnées sur place !", ColorConstants.Red);
          player.oid.LoginCreature.Gold = 0;
          player.StripPlayerOfCraftResources();
        });*/
      }
    }
    public static void SetupPCCorpse(NwCreature oPCCorpse)
    {
      Task settingUpCorpse = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        oPCCorpse.ApplyEffect(EffectDuration.Instant, Effect.Death());
        NwWaypoint wp = NwWaypoint.Create("NW_WAYPOINT001", oPCCorpse.Location, false, $"wp_pccorpse_{oPCCorpse.GetObjectVariable<LocalVariableInt>("_PC_ID").Value}");
        await NwTask.Delay(TimeSpan.FromSeconds(0.8));
        oPCCorpse.VisibilityOverride = Anvil.Services.VisibilityMode.Default;
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        oPCCorpse.Tag = "pccorpse";
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        wp.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(b => b.Tag == "BodyBag").Tag = "pccorpse_bodybag";
        scheduler.Schedule(() => wp.Destroy(), TimeSpan.FromSeconds(0.1));
      });
    }
    public static void DeletePlayerCorpseFromDatabase(int characterId)
    {
      SqLiteUtils.DeletionQuery("playerDeathCorpses",
          new Dictionary<string, string>() { { "characterId", characterId.ToString() } });
    }
    public static void SavePlayerCorpseToDatabase(int characterId, NwCreature deathCorpse)
    {
      SqLiteUtils.InsertQuery("playerDeathCorpses",
          new List<string[]>() { new string[] { "characterId", characterId.ToString() }, new string[] { "deathCorpse", deathCorpse.Serialize().ToBase64EncodedString() }, new string[] { "location", SqLiteUtils.SerializeLocation(deathCorpse.Location) } });
    }
    public partial class Player
    {
      public void CreatePlayerCorpse(Location deathLocation)
      {
        NwCreature oPCCorpse = oid.LoginCreature.Clone(deathLocation, "pccorpse");

        foreach (NwItem item in oPCCorpse.Inventory.Items)
          item.Destroy();

        NwItem oCorpseItem = NwItem.Create("item_pccorpse", oPCCorpse.Location);
        oPCCorpse.AcquireItem(oCorpseItem);
       //oCorpseItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;

        oPCCorpse.Lootable = true;
        oPCCorpse.Name = $"Corps inconscient de {oid.LoginCreature.Name}";
        oPCCorpse.Description = $"Corps inconscient de {oid.LoginCreature.Name}. \n\n\n Allez savoir combien de temps il va tenir dans cet état.";
        oPCCorpse.VisibilityOverride = Anvil.Services.VisibilityMode.Hidden;

        oPCCorpse.GetObjectVariable<LocalVariableInt>("_PC_ID").Value = characterId;

        for (int i = 0; i <= (int)InventorySlot.Bolts; i++)
          if (oPCCorpse.GetItemInSlot((InventorySlot)i) != null)
            oPCCorpse.GetItemInSlot((InventorySlot)i).Droppable = false;

        oCorpseItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value = characterId;
        oCorpseItem.Name = $"Corps inconscient de {oid.LoginCreature.Name}";
        oCorpseItem.Description = $"Corps inconscient de {oid.LoginCreature.Name}\n\n\n Pas très ragoûtant. Allez savoir combien de temps il va tenir avant de se lâcher.";
        oCorpseItem.Droppable = true;

        oCorpseItem.GetObjectVariable<LocalVariableString>("_SERIALIZED_CORPSE").Value = oPCCorpse.Serialize().ToBase64EncodedString();
        deathCorpse = oPCCorpse;

        SavePlayerCorpseToDatabase(characterId, deathCorpse);

        SetupPCCorpse(oPCCorpse);

        LogUtils.LogMessage($"Corpse {oPCCorpse.Name} created", LogUtils.LogType.PlayerDeath);
      }
      public void StripPlayerOfCraftResources()
      {
        LogUtils.LogMessage($"{oid.LoginCreature.Name} dead. Stripping him of craft resources", LogUtils.LogType.PlayerDeath);
        foreach (NwItem oItem in oid.LoginCreature.Inventory.Items.Where(i => i.Tag == "craft_resource" || i.Tag == "blueprint"))
        {
          LogUtils.LogMessage($"{oItem.Name} stripped", LogUtils.LogType.PlayerDeath);
          oItem.Clone(deathCorpse).Droppable = true;
          oItem.Destroy();
        }
      }
      public void SendPlayerToLimbo()
      {
        oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRestorationGreater));
        oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Resurrection());
        oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Heal(oid.LoginCreature.MaxHP));
      }
      public void Respawn()
      {
        DestroyPlayerCorpse();
        CreatureUtils.InitThreatRange(oid.LoginCreature);
        oid.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>("WP_RESPAWN_DISPENSAIRE").FirstOrDefault()?.Location;

        bankGold -= 50;
        oid.SendServerMessage("Afin de vous remettre sur pied, les 'soigneurs' ont demandé à la banque de prélever 50 pièces d'or sur votre compte.");
      }
      public void DestroyPlayerCorpse()
      {
        DeletePlayerCorpseFromDatabase(characterId);

        NwCreature oCorpse = NwObject.FindObjectsWithTag<NwCreature>("pccorpse").Where(c => c.GetObjectVariable<LocalVariableInt>("_PC_ID").Value == characterId).FirstOrDefault();
        oCorpse?.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(o => o.Tag == "pccorpse_bodybag")?.Destroy();
        oCorpse?.Destroy();
        NwObject.FindObjectsWithTag<NwItem>("item_pccorpse").FirstOrDefault(c => c.GetObjectVariable<LocalVariableInt>("_PC_ID").Value == characterId)?.Destroy();
      }
    }
  }
}
