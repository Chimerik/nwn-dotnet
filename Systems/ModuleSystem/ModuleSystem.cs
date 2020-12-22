using NWN.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using Microsoft.Data.Sqlite;
using Google.Cloud.Translation.V2;
using System.Runtime.CompilerServices;

namespace NWN.Systems
{
  class ModuleSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { "module_heartbeat", HandleModuleHeartBeat },
      { "event_moduleload", HandleModuleLoad },
      { "_event_effects", EventEffects },
      { "onent_theater_sc", HandleEnterTheaterScene },
      { "onex_theater_sc", HandleExitTheaterScene },
      { "ondeath_wraith", HandleWraithDeath },
      { "ondeath_quaranti", HandleQuarantineWereDeath },
    }.Concat(Systems.LootSystem.Register)
     .Concat(Systems.PlayerSystem.Register)
     .Concat(Systems.ChatSystem.Register)
     .Concat(Systems.SpellSystem.Register)
     .Concat(Systems.ItemSystem.Register)
     .Concat(PlaceableSystem.Register)
     .Concat(Systems.CollectSystem.Register)
     .Concat(Systems.DialogSystem.Register)
     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    public static string database = Environment.GetEnvironmentVariable("DB_NAME");
    public static string db_path = "Data Source=" + Environment.GetEnvironmentVariable("DB_DIRECTORY");
    public static TranslationClient googleTranslationClient = TranslationClient.Create();
    public static Module module;
    private static int HandleModuleLoad(uint oidSelf)
    {
      module = new Module(NWScript.GetModule());
      
      return -1;
    }
    private static int HandleModuleHeartBeat(uint oidSelf)
    {
      foreach (string command in module.botAsyncCommandList)
        BotAsyncCommandSystem.ProcessBotAsyncCommand(command);

      module.botAsyncCommandList.Clear();

      return 0;
    }
    private static int EventEffects(uint oidSelf)
    {
      string current_event = EventsPlugin.GetCurrentEvent();
      int effectType = int.Parse(EventsPlugin.GetEventData("TYPE"));
      int effectIntParam1 = int.Parse(EventsPlugin.GetEventData("INT_PARAM_1"));

      if (current_event == "NWNX_ON_EFFECT_REMOVED_AFTER")
      {
        if (EventsPlugin.GetEventData("CUSTOM_TAG") == "lycan_curse")
        {
          PlayerSystem.Player player;
          if (PlayerSystem.Players.TryGetValue(oidSelf, out player))
          {
            player.RemoveLycanCurse();
          }
        }
        else if (effectType == NWScript.EFFECT_TYPE_ABILITY_INCREASE && effectIntParam1 == NWScript.ABILITY_STRENGTH)
        {
          if (NWScript.GetMovementRate(oidSelf) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) >= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);
        }
        else if (effectType == NWScript.EFFECT_TYPE_ABILITY_DECREASE && effectIntParam1 == NWScript.ABILITY_STRENGTH)
        {
          if (NWScript.GetMovementRate(oidSelf) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DEFAULT);
        }
      }
      else if (current_event == "NWNX_ON_EFFECT_APPLIED_AFTER")
      {
        if (effectType == NWScript.EFFECT_TYPE_ABILITY_INCREASE && effectIntParam1 == NWScript.ABILITY_STRENGTH)
        {
          if (NWScript.GetMovementRate(oidSelf) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_DEFAULT);
        }
        else if (effectType == NWScript.EFFECT_TYPE_ABILITY_DECREASE && effectIntParam1 == NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))
        {
          if (NWScript.GetMovementRate(oidSelf) != CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE)
            if (NWScript.GetWeight(oidSelf) >= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(oidSelf, NWScript.ABILITY_STRENGTH))))
              CreaturePlugin.SetMovementRate(oidSelf, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE);
        }
      }

      return 0;
    }
    private static int HandleEnterTheaterScene(uint oidSelf)
    {
      NWScript.SetObjectVisualTransform(NWScript.GetEnteringObject(), NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 2.01f);
      return 0;
    }
    
    private static int HandleExitTheaterScene(uint oidSelf)
    {
      NWScript.SetObjectVisualTransform(NWScript.GetExitingObject(), NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 0);
      return 0;
    }
    private static int HandleWraithDeath(uint oidSelf)
    {
      NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, "similisse_wraith", NWScript.GetLocation(NWScript.GetNearestObjectByTag("creature_spawn", oidSelf)));
      NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, "similisse_wraith", NWScript.GetLocation(NWScript.GetNearestObjectByTag("creature_spawn", oidSelf)));
      return 0;
    }
    private static int HandleQuarantineWereDeath(uint oidSelf)
    {

      NWScript.CreateObject(NWScript.OBJECT_TYPE_CREATURE, NWScript.GetResRef(oidSelf), NWScript.GetLocation(NWScript.GetNearestObjectByTag("creature_spawn", oidSelf)));
      return 0;
    }
  }
}
