using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using NWN.Core;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static void HandleSpawnSpecificBehaviour(NwCreature creature)
    {
      switch(creature.Tag)
      {
        case "statue_tiamat":

          creature.OnConversation += PlaceableSystem.HandleCancelStatueConversation;

          Effect eff = Effect.CutsceneGhost();
          eff.SubType = EffectSubType.Supernatural;
          eff.Tag = "_GHOST_EFFECT";

          creature.ApplyEffect(EffectDuration.Permanent, eff);

          creature.MouseCursor = MouseCursor.Walk;
          creature.AiLevel = AiLevel.VeryLow;

          Task waitForCreatureToSpawn = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.5));
            await creature.PlayAnimation(Animation.LoopingPause, 0.0001f, false, TimeSpan.FromDays(365));
          });

          return;

        case "statue_reptilienne":

          creature.OnConversation += PlaceableSystem.HandleCancelStatueConversation;

          Effect eff2 = Effect.LinkEffects(Effect.CutsceneGhost(), Effect.VisualEffect((VfxType)927));
          eff2.SubType = EffectSubType.Supernatural;
          eff2.Tag = "_STATUE_EFFECT";
          creature.ApplyEffect(EffectDuration.Permanent, eff2);

          creature.MouseCursor = MouseCursor.Walk;
          creature.AiLevel = AiLevel.VeryLow;
          creature.HighlightColor = ColorConstants.Black;

          Task waitForCreatureToSpawn2 = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.5));
            await creature.PlayAnimation((Animation)creature.GetObjectVariable<LocalVariableInt>("animation").Value, 5, false, TimeSpan.FromDays(365));
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));

            Effect freeze = Effect.VisualEffect(VfxType.DurFreezeAnimation);
            freeze.Tag = "_FREEZE_EFFECT";
            creature.ApplyEffect(EffectDuration.Permanent, freeze);
          });

          return;
      }

      switch (creature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value)
      {
        case "npc":
          SetNPCEvents(creature);
          creature.AiLevel = AiLevel.VeryLow;
          break;
        case "civilian":
        case "plage":
        case "cave":
        case "city":
        case "generic":
          Appearance2da.appearanceTable.SetRandomAppearance(creature);
          creature.AiLevel = AiLevel.VeryLow;
          creature.ActionRandomWalk();
          break;
        default:
          HandleMobSpecificBehaviour(creature);
          break;
      }
    }
    private static void HandleMobSpecificBehaviour(NwCreature creature)
    {
      switch (creature.Tag)
      {
        case "ratgarou":
        case "wereboar":
          creature.OnDeath += HandleWereInfiniteSpawn;
          break;
        case "sim_wraith":
          creature.OnDeath += HandleWraithInfiniteSpawn;
          break;
        default:
          creature.AiLevel = AiLevel.Low;
          creature.OnDeath += LootSystem.HandleLoot;
          break;
      }

      creature.OnHeartbeat += CheckDistanceFromSpawn;
    }

    private static async void CheckDistanceFromSpawn(CreatureEvents.OnHeartbeat onHB)
    {
      Log.Info("HB spawn distance check on");
      if (onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value.DistanceSquared(onHB.Creature) < 1600)
      {
        Log.Info("HB spawn distance check off (NO RESET)");
        return;
      }

      onHB.Creature.AiLevel = AiLevel.VeryLow;
      await onHB.Creature.ClearActionQueue();
      await onHB.Creature.ActionForceMoveTo(onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value, true, 0, TimeSpan.FromSeconds(30));

      Effect regen = NWScript.EffectRunScript("", "mobReset_off", "mobReset_on", 1);
      regen.Tag = "mob_reset_regen";
      regen.SubType = EffectSubType.Supernatural;
      onHB.Creature.ApplyEffect(EffectDuration.Permanent, regen);

      Log.Info("HB spawn distance check off (RESETTING)");
    }

    /*private static void OnDeathSpawnNPCWaypoint(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint waypoint = NwWaypoint.Create(onDeath.KilledCreature.GetObjectVariable<LocalVariableString>("_WAYPOINT_TEMPLATE"), onDeath.KilledCreature.GetObjectVariable<LocalVariableLocation>("_SPAWN_LOCATION").Value);
      waypoint.GetObjectVariable<LocalVariableString>("_CREATURE_TEMPLATE").Value = onDeath.KilledCreature.ResRef;
    }*/
    private static void HandleWereInfiniteSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint wp = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      NwCreature.Create(onDeath.KilledCreature.ResRef, wp.Location).OnDeath += HandleWereInfiniteSpawn;
    }
    private static void HandleWraithInfiniteSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint wp = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      NwCreature.Create(onDeath.KilledCreature.ResRef, wp.Location).OnDeath += HandleWraithInfiniteSpawn;
      NwCreature.Create(onDeath.KilledCreature.ResRef, wp.Location).OnDeath += HandleWraithInfiniteSpawn;
    }
    private static void SetNPCEvents(NwCreature creature)
    {
      //creature.OnDeath += OnDeathSpawnNPCWaypoint;

      switch (creature.Tag)
      {
        case "bank_npc":
          creature.OnConversation += DialogSystem.StartBankerDialog;
          break;
        case "blacksmith":
          creature.OnConversation += DialogSystem.StartBlacksmithDialog;
          break;
        case "woodworker":
          creature.OnConversation += DialogSystem.StartWoodworkerDialog;
          break;
        case "tanneur":
          creature.OnConversation += DialogSystem.StartTanneurDialog;
          break;
        case "le_bibliothecaire":
          creature.OnConversation += DialogSystem.StartBibliothecaireDialog;
          break;
        case "jukebox":
          creature.OnConversation += DialogSystem.StartJukeboxDialog;
          break;
        case "jukebox2":
          creature.OnConversation += DialogSystem.StartBardeDragonDialog;
          break;
        case "rumors":
          creature.OnConversation += DialogSystem.StartRumorsDialog;
          break;
        case "tribunal_hotesse":
          creature.OnConversation += DialogSystem.StartTribunalShopDialog;
          break;
        case "pve_arena_host":
          creature.OnConversation += DialogSystem.StartPvEArenaHostDialog;
          break;
        case "bal_system":
          creature.OnConversation += DialogSystem.StartMessengerDialog;
          break;
        case "storage_npc":
          creature.OnConversation += DialogSystem.StartStorageDialog;
          break;
      }
    }
  }
}
