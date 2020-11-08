using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.CollectSystem;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class ItemSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_equip_items_before", HandleBeforeEquipItem },
            { "event_unequip_items_before", HandleBeforeUnequipItem },
            { "event_validate_equip_items_before", HandleBeforeValidatingEquipItem},
            { "event_use_item_before", HandleBeforeUseItem},
    }; 
    private static int HandleBeforeEquipItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));
        int iSlot = int.Parse(EventsPlugin.GetEventData("SLOT"));
        var oUnequip = NWScript.GetItemInSlot(iSlot, player.oid);

        if (NWScript.GetIsObjectValid(oUnequip) == 1 && ObjectPlugin.CheckFit(player.oid, NWScript.GetBaseItemType(oUnequip)) == 0)
        {
          NWScript.SendMessageToPC(player.oid, "Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          EventsPlugin.SkipEvent();
          return 0;
        }
      }
      return 0;
    }
    private static int HandleBeforeUnequipItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));
        if (ObjectPlugin.CheckFit(player.oid, NWScript.GetBaseItemType(oItem)) == 0)
        {
          NWScript.SendMessageToPC(player.oid, "Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          EventsPlugin.SkipEvent();
          return 0;
        }
      }
      return 0;
    }
    private static int HandleBeforeValidatingEquipItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM_OBJECT_ID"));
        
        switch (NWScript.GetTag(oItem))
        {
          case "extracteur":
            int value;
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.StripMiner)), out value)) 
            {
              int itemLevel = NWScript.GetLocalInt(oItem, "_ITEM_LEVEL");
              
              if (itemLevel > value)
              {
                EventsPlugin.SetEventResult("0");
                EventsPlugin.SkipEvent();

                if(EventsPlugin.GetCurrentEvent() == "NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE")
                  NWScript.SendMessageToPC(player.oid, $"Le niveau {itemLevel} de maîtrise des extracteur de roche est requis pour pouvoir utiliser cet outil.");
              }          
            }
            else
            {
              EventsPlugin.SetEventResult("0");
              EventsPlugin.SkipEvent();

              if (EventsPlugin.GetCurrentEvent() == "NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE")
                NWScript.SendMessageToPC(player.oid, $"Le don maîtrise des extracteur de roche est requis pour pouvoir utiliser cet outil.");
            }
              break;
        }
      }
      return 0;
    }
    private static int HandleBeforeUseItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM_OBJECT_ID"));
        uint oTarget;

        switch (NWScript.GetTag(oItem))
        {
          case "skillbook":
            EventsPlugin.SkipEvent();
            HandleSkillBookActivate(oItem, player);
            break;
          case "blueprint":
            EventsPlugin.SkipEvent();
            oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
            HandleBlueprintActivate(oItem, player, oTarget);
            break;
          case "loot_saver":
            EventsPlugin.SkipEvent();
            oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_OBJECT_ID"));
            HandleLootSaverActivate(player, oTarget);
            break;
        }
      }
      return 0;
    }
  }
}
