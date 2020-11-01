using NWN.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using Microsoft.Data.Sqlite;
using Google.Cloud.Translation.V2;

namespace NWN.Systems
{
  class ModuleSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { "module_heartbeat", HandleModuleHeartBeat },
      { "event_moduleload", HandleModuleLoad },
      { "x2_mod_def_act", HandleActivateItem },
      //  { "event_mouse_clic", EventMouseClick },
      { "event_potager", EventPotager },
      { "_event_effects", EventEffects },
    }.Concat(Systems.LootSystem.Register)
     .Concat(Systems.PlayerSystem.Register)
     .Concat(Systems.ChatSystem.Register)
     .Concat(Systems.SpellSystem.Register)
     .Concat(Systems.ItemSystem.Register)
     .Concat(PlaceableSystem.Register)
     .Concat(Systems.CollectSystem.Register)
     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    public static string database = Environment.GetEnvironmentVariable("DN_NAME");
    public static TranslationClient googleTranslationClient = TranslationClient.Create();
    public static Module module;
    private static int HandleModuleLoad(uint oidSelf)
    {
      module = new Module(NWScript.GetModule());
      
      return -1;
    }
    private static int HandleModuleHeartBeat(uint oidSelf)
    {
      if (module.reboot)
      {
        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          if (NWScript.GetIsDM(PlayerListEntry.Key) != 1)
            NWScript.BootPC(PlayerListEntry.Key, "Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
        }
      }

      return 0;
    }
    private static int HandleActivateItem(uint oidSelf)
    {
      var oItem = NWScript.GetItemActivated();
      var oActivator = NWScript.GetItemActivator();
      var oTarget = NWScript.GetItemActivatedTarget();
      var tag = NWScript.GetTag(oItem);

      Func<uint, uint, uint, int> handler;
      if (ActivateItemHandlers.Register.TryGetValue(tag, out handler))
      {
        try
        {
          return handler.Invoke(oItem, oActivator, oTarget);
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }

      return 0;
    }
    private static int EventPotager(uint oidSelf)
    {
      Garden oGarden;
      if (Garden.Potagers.TryGetValue(NWScript.GetLocalInt(oidSelf, "id"), out oGarden))
      {
        oGarden.PlanterFruit(EventsPlugin.GetEventData("FRUIT_NAME"), EventsPlugin.GetEventData("FRUIT_TAG"));
      }

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
  }
}
