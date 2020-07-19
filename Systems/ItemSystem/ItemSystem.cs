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
            { "event_items", ItemEquip },
    };
    private static int ItemEquip(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        string current_event = NWNX.Events.GetCurrentEvent();

        if (current_event == "NWNX_ON_ITEM_EQUIP_BEFORE")
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
        else if (current_event == "NWNX_ON_ITEM_UNEQUIP_BEFORE")
        {
          NWItem oItem = NWNX.Object.StringToObject(NWNX.Events.GetEventData("ITEM")).AsItem();
          if (NWNX.Object.CheckFit(player, (int)oItem.BaseItemType) == 0)
          {
            player.SendMessage("Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
            NWNX.Events.SkipEvent();
            return Entrypoints.SCRIPT_HANDLED;
          }
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
