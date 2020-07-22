using System;
using System.Collections.Generic;
using NWN.Enums;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class ItemSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_equip_items_before", HandleBeforeEquipItem },
            { "event_unequip_items_before", HandleBeforeUnequipItem },
    };
    private static int HandleBeforeEquipItem(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWNX.Object.StringToObject(NWNX.Events.GetEventData("ITEM"));
        int iSlot = int.Parse(NWNX.Events.GetEventData("SLOT"));
        NWItem oUnequip = NWScript.GetItemInSlot((InventorySlot)iSlot, player).AsItem();

        if (oUnequip.IsValid && NWNX.Object.CheckFit(player, (int)oUnequip.BaseItemType) == 0)
        {
          player.SendMessage("Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          NWNX.Events.SkipEvent();
          return Entrypoints.SCRIPT_HANDLED;
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleBeforeUnequipItem(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWItem oItem = NWNX.Object.StringToObject(NWNX.Events.GetEventData("ITEM")).AsItem();
        if (NWNX.Object.CheckFit(player, (int)oItem.BaseItemType) == 0)
        {
          player.SendMessage("Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          NWNX.Events.SkipEvent();
          return Entrypoints.SCRIPT_HANDLED;
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
