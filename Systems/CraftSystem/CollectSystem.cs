﻿using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Blueprint;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class CollectSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_mining_cycle_cancel_before", HandleBeforeMiningCycleCancel },
            { "on_mining_cycle_complete", HandleAfterMiningCycleComplete },
    };

    public static Dictionary<BlueprintType, Blueprint> blueprintDictionnary = new Dictionary<BlueprintType, Blueprint>();
    private static int HandleBeforeMiningCycleCancel(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoActionOnMiningCycleCancelled();
      }

      return 0;
    }
    private static int HandleAfterMiningCycleComplete(uint oidSelf)
    {
      NWScript.SendMessageToPC(oidSelf, "Mining cycle completed !");

      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoActionOnMiningCycleCompleted();
      }

      return 0;
    }    
    public static void StartMiningCycle(Player player, uint rock, Action cancelCallback, Action completeCallback)
    {
      player.OnMiningCycleCancelled = cancelCallback;
      player.OnMiningCycleCompleted = completeCallback;

      var miningStriper = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);
      float cycleDuration = 180.0f;

      if (NWScript.GetIsObjectValid(miningStriper) == 1) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      {
        cycleDuration = cycleDuration - (cycleDuration * NWScript.GetLocalInt(miningStriper, "_ITEM_LEVEL") * 2 / 100);
      }

      Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_DISINTEGRATE, miningStriper, 1);
      eRay = NWScript.TagEffect(eRay, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, rock, cycleDuration);
      
      PlayerPlugin.StartGuiTimingBar(player.oid, cycleDuration, "on_mining_cycle_complete");

      EventsPlugin.AddObjectToDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
    }
    public static void RemoveMiningCycleCallbacks(Player player)
    {
      player.OnMiningCycleCancelled = () => { };
      player.OnMiningCycleCompleted = () => { };
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
    }
    public static void AddCraftedItemProperties(uint craftedItem, Blueprint blueprint, string material)
    {
      NWScript.SetName(craftedItem, NWScript.GetName(craftedItem) + " en " + material);
      NWScript.SetLocalString(craftedItem, "_ITEM_MATERIAL", material);

      foreach (ItemProperty ip in GetCraftItemProperties(GetMineralTypeFromName(material), ItemSystem.GetItemCategory(craftedItem)))
      {
        NWScript.AddItemProperty(NWScript.DURATION_TYPE_PERMANENT, ip, craftedItem);
      }
    }
    public static Boolean IsItemCraftMaterial(string itemTag)
    {
      if (GetOreTypeFromName(itemTag) != OreType.Invalid || GetMineralTypeFromName(itemTag) != MineralType.Invalid)
        return true;

      return false;
    }
    public static string GetCraftMaterialItemTemplate(string itemTag)
    {
      if (GetOreTypeFromName(itemTag) != OreType.Invalid)
        return "ore";
      else if (GetMineralTypeFromName(itemTag) != MineralType.Invalid)
        return "mineral";

      Utils.LogMessageToDMs($"Could not determiner item template for tag : {itemTag}");
      return "";
    }
  }
}
