using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.Enums.VisualEffect;
using NWN.NWNX;
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
    private static int HandleBeforeMiningCycleCancel(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoActionOnMiningCycleCancelled();
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleAfterMiningCycleComplete(uint oidSelf)
    {
      NWScript.SendMessageToPC(oidSelf, "Mining cycle completed !");

      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoActionOnMiningCycleCompleted();
      }

      return Entrypoints.SCRIPT_HANDLED;
    }    
    public static void StartMiningCycle(PlayerSystem.Player player, NWPlaceable rock, Action cancelCallback, Action completeCallback)
    {
      player.OnMiningCycleCancelled = cancelCallback;
      player.OnMiningCycleCompleted = completeCallback;

      NWItem miningStriper = player.Equipped[InventorySlot.RightHand];
      float cycleDuration = 180.0f;

      if (miningStriper.IsValid) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      {
        cycleDuration = cycleDuration - (cycleDuration * miningStriper.Locals.Int.Get("_ITEM_LEVEL") * 2 / 100);
      }

      Effect eRay = NWScript.EffectBeam(Beam.Disintegrate, miningStriper, 1);
      eRay = NWScript.TagEffect(eRay, $"_{player.CDKey}_MINING_BEAM");
      rock.ApplyEffect(DurationType.Temporary, eRay, cycleDuration);

      NWNX.Player.StartGuiTimingBar(player, cycleDuration, "on_mining_cycle_complete");

      NWNX.Events.AddObjectToDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", player);
      NWNX.Events.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", player);
    }
    public static void RemoveMiningCycleCallbacks(PlayerSystem.Player player)
    {
      player.OnMiningCycleCancelled = () => { };
      player.OnMiningCycleCompleted = () => { };
      NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", player);
      NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", player);
      NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", player);
    }
    public static void AddCraftedItemProperties(uint craftedItem, Blueprint blueprint, int level)
    {
      foreach (ItemProperty ip in itemPropertiesDictionnary[level])
        NWScript.AddItemProperty(DurationType.Permanent, ip, craftedItem);
      
    }
  }
}
