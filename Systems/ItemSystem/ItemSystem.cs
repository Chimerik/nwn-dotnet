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
            { "event_pccorpse_remove_item_after", HandleAfterItemRemovedFromPCCorpse },
            { "event_pccorpse_add_item_after", HandleAfterItemAddedToPCCorpse },
            { "event_refinery_add_item_before", HandleBeforeItemAddedToRefinery },
            { "refinery_add_item", HandleItemAddedToRefinery },
            { "refinery_close", HandleRefineryClose },
            {"event_validate_equip_items_before", HandleBeforeValidatingEquipItem},
    };
    private static int HandleBeforeEquipItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ITEM"));
        int iSlot = int.Parse(EventsPlugin.GetEventData("SLOT"));
        var oUnequip = NWScript.GetItemInSlot(iSlot, player.oid);

        if (NWScript.GetIsObjectValid(oUnequip) == 1 && ObjectPlugin.CheckFit(player.oid, NWScript.GetBaseItemType(oUnequip)) == 0)
        {
          NWScript.SendMessageToPC(player.oid, "Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          EventsPlugin.SkipEvent();
          return 1;
        }
      }
      return 1;
    }
    private static int HandleBeforeUnequipItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ITEM"));
        if (ObjectPlugin.CheckFit(player.oid, NWScript.GetBaseItemType(oItem)) == 0)
        {
          NWScript.SendMessageToPC(player.oid, "Attention, votre inventaire est plein. Déséquipper cet objet risquerait de vous le faire perdre !");
          EventsPlugin.SkipEvent();
          return 1;
        }
      }
      return 1;
    }
    private static int HandleBeforeValidatingEquipItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        var oItem = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ITEM_OBJECT_ID"));
        
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
      return 1;
    }
    private static int HandleBeforeItemAddedToRefinery(uint oidSelf)
    {
      var oItem = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ITEM"));

      if (NWScript.GetTag(oItem) != "ore" || NWScript.GetTag(oItem) != "mineral")
      {
        EventsPlugin.SkipEvent();
        NWScript.SpeakString("Seul le minerai peut être raffiné dans la fonderie.");
      }
      return 1;
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

      return 1;
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

      return 1;
    }
    private static int HandleAfterItemAddedToPCCorpse(uint oidSelf)
    {
      var oItem = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ITEM"));

      switch (NWScript.GetTag(oItem)) 
      {
        case "pccorpse":
          // TODO : mettre à jour le cadavre serialisé en BDD
          break;
      }
      return 1;
    }
    private static int HandleAfterItemRemovedFromPCCorpse(uint oidSelf)
    {
      var oItem = ObjectPlugin.StringToObject(EventsPlugin.GetEventData("ITEM"));

      switch (NWScript.GetTag(oItem)) // TODO : Ca va pas du tout. A tester et à revoir
      {
        case "pccorpse":
          if (NWScript.GetTag(oItem) == "item_pccorpse")
          {
            // TODO : détruire l'objet corps en BDD également where _PC_ID
            NWScript.DestroyObject(oItem);
          }
          else
          {
            // TODO : mettre à jour le cadavre serialisé en BDD
          }
          break;
      }

      if (NWScript.GetIsPC(oidSelf) == 1) // TODO : y a un truc qui va pas, logique à retravailler (abonner le pj et le cadavre uniquement aux événements respectifs)
      {
        var player = oidSelf;
        var oPCCorpse = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "pccorpse", NWScript.GetLocation(player));
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after", oPCCorpse);
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_ADD_ITEM_AFTER", "event_pccorpse_add_item_after", oPCCorpse);

        int PlayerId = ObjectPlugin.GetInt(oItem, "_PC_ID");
        //oPCCorpse.Name = $"Cadavre de {NWScript.GetName(player.oid)}"; TODO : chopper le nom du PJ en BDD à partir de son ID
        //oPCCorpse.Description = $"Cadavre de {NWScript.GetName(player.oid)}";
        NWScript.SetLocalInt(oPCCorpse, "_PC_ID", PlayerId);
        ObjectPlugin.AcquireItem(oPCCorpse, oItem);

        // TODO : enregistrer oPCCorpse en BDD
      }
      return 1;
    }
  }
}
