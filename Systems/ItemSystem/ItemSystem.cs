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
            { "event_refinery_add_item_before", HandleBeforeItemAddedToRefinery },
            { "refinery_add_item", HandleItemAddedToRefinery },
            { "refinery_close", HandleRefineryClose },
            { "event_validate_equip_items_before", HandleBeforeValidatingEquipItem},
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
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1132)), out value)) // TODO : changer la valeur par un enum des feats customs // strip miner 
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
    private static int HandleBeforeItemAddedToRefinery(uint oidSelf)
    {
      var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));

      if (NWScript.GetTag(oItem) != "ore" || NWScript.GetTag(oItem) != "mineral")
      {
        EventsPlugin.SkipEvent();
        NWScript.SpeakString("Seul le minerai peut être raffiné dans la fonderie.");
      }
      return 0;
    }
    private static int HandleItemAddedToRefinery(uint oidSelf)
    {
      if(NWScript.GetInventoryDisturbType() == NWScript.INVENTORY_DISTURB_TYPE_ADDED)
      {
        var item = NWScript.GetInventoryDisturbItem();

        if (NWScript.GetTag(item) == "ore")
        {
          PlayerSystem.Player player;
          if (Players.TryGetValue(NWScript.GetLastDisturbed(), out player))
          {
            string reprocessingData = $"{NWScript.GetName(item)} : Efficacité raffinage -30 % (base fonderie)";
            
            int value;
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1133)), out value)) // TODO : changer la valeur par un enum des feats customs reprocessing
              reprocessingData += $"\n x1.{3*value} (Raffinage)";

            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1134)), out value)) // TODO : changer la valeur par un enum des feats customs reprocessing efficiency
              reprocessingData += $"\n x1.{2 * value} (Raffinage efficace)";

            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1135)), out value)) // TODO : reprocessing specialty
              reprocessingData += $"\n x1.{2 * value} (Raffinage {NWScript.GetName(item)})";

            float connectionsLevel;
            if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1136)), out connectionsLevel)) // TODO : changer la valeur par un enum des feats customs // connections
              reprocessingData += $"\n x{1.00 - connectionsLevel / 100} (Raffinage {NWScript.GetName(item)})";

            NWScript.SendMessageToPC(player.oid, reprocessingData);
          }
        }
      }

      return 0;
    }
    private static int HandleRefineryClose(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(NWScript.GetLastClosedBy(), out player))
      {
        var fonderie = oidSelf;
        float reprocessingEfficiency = 0.3f;

        float value;
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1140)), out value)) // TODO : changer la valeur par un enum des feats customs reprocessing
          reprocessingEfficiency += reprocessingEfficiency + 3 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1141)), out value)) // TODO : changer la valeur par un enum des feats customs reprocessing efficiency
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, 1142)), out value)) // TODO : changer la valeur par un enum des feats customs connections
          reprocessingEfficiency += reprocessingEfficiency + 1 * value / 100;
        
        var ore = NWScript.GetFirstItemInInventory(fonderie);
        while (NWScript.GetIsObjectValid(ore) == 1)
        {
          if(NWScript.GetTag(ore) == "ore")
          {
            if (NWScript.GetItemStackSize(ore) > 100)
            {
              CollectSystem.Ore processedOre;
              if (CollectSystem.oresDictionnary.TryGetValue(GetOreTypeFromName(NWScript.GetName(ore)), out processedOre))
              {
                if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)processedOre.feat)), out value))
                  reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

                foreach (KeyValuePair<MineralType, float> mineralKeyValuePair in processedOre.mineralsDictionnary)
                {
                  var mineral = NWScript.CreateItemOnObject("mineral", player.oid, (int)((NWScript.GetItemStackSize(ore) * mineralKeyValuePair.Value * (int)reprocessingEfficiency)));
                  NWScript.SetName(mineral, GetNameFromMineralType(mineralKeyValuePair.Key));
                  NWScript.SetLocalInt(mineral, "DROPS_ON_DEATH", 1);
                }

                NWScript.DestroyObject(ore);
              }
            }
            else
              NWScript.SendMessageToPC(player.oid, $"Ce lot de {NWScript.GetName(ore)} n'a pas pu être raffiné. Un minimum de 100 unités est nécessaire pour le bon fonctionnement de la fonderie.");
          }

          ore = NWScript.GetNextItemInInventory(fonderie);
        }
        
      }

      return 0;
    }
  }
}
