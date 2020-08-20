using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class ItemSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_equip_items_before", HandleBeforeEquipItem },
            { "event_unequip_items_before", HandleBeforeUnequipItem },
            { "event_inventory_remove_item_after", HandleAfterItemRemovedFromInventory },
            { "event_inventory_add_item_after", HandleAfterItemAddedToInventory },
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
      PlayerSystem.Player player;
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
    private static int HandleAfterItemRemovedFromInventory(uint oidSelf)
    {
      NWItem oItem = NWNX.Object.StringToObject(NWNX.Events.GetEventData("ITEM")).AsItem();

      if(NWScript.GetMovementRate(oidSelf) == (int)MovementRate.Immobile)
        if (NWScript.GetWeight(oidSelf) <= int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oidSelf.AsCreature().Ability[Ability.Strength].Total)))
          NWNX.Creature.SetMovementRate(oidSelf, MovementRate.Default);

      switch (oItem.Tag)
      {
        case "pccorpse":
          if (oItem.Tag == "item_pccorpse")
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

      if(oidSelf.AsCreature().IsPC)
      {
        NWPlayer player = oidSelf.AsPlayer();
        NWPlaceable oPCCorpse = NWScript.CreateObject(ObjectType.Placeable, "pccorpse", player.Location).AsPlaceable();
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_inventory_remove_item_after", oPCCorpse);
        NWNX.Events.AddObjectToDispatchList("NWNX_ON_INVENTORY_ADD_ITEM_AFTER", "event_inventory_add_item_after", oPCCorpse);

        int PlayerId = NWNX.Object.GetInt(oItem, "_PC_ID");
        //oPCCorpse.Name = $"Cadavre de {player.Name}"; TODO : chopper le nom du PJ en BDD à partir de son ID
        //oPCCorpse.Description = $"Cadavre de {player.Name}";
        oPCCorpse.Locals.Int.Set("_PC_ID", PlayerId);
        NWNX.Object.AcquireItem(oPCCorpse, oItem);

        // TODO : enregistrer oPCCorpse en BDD
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleAfterItemAddedToInventory(uint oidSelf)
    {
      NWItem oItem = NWNX.Object.StringToObject(NWNX.Events.GetEventData("ITEM")).AsItem();

      if (NWScript.GetWeight(oidSelf) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", oidSelf.AsCreature().Ability[Ability.Strength].Total)))
        NWNX.Creature.SetMovementRate(oidSelf, MovementRate.Immobile);

      switch (oItem.Tag)
      {
        case "pccorpse":
            // TODO : mettre à jour le cadavre serialisé en BDD
          break;
      }
      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleBeforeValidatingEquipItem(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        NWItem oItem = NWNX.Object.StringToObject(NWNX.Events.GetEventData("ITEM_OBJECT_ID")).AsItem();
        
        switch (oItem.Tag)
        {
          case "extracteur":
            int value;
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.StripMinerMastery)), out value))
            {
              int itemLevel = oItem.Locals.Int.Get("_ITEM_LEVEL");
              
              if (itemLevel > value)
              {
                NWNX.Events.SetEventResult("0");
                NWNX.Events.SkipEvent();

                if(NWNX.Events.GetCurrentEvent() == "NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE")
                  player.SendMessage($"Le niveau {itemLevel} de maîtrise des extracteur de roche est requis pour pouvoir utiliser cet outil.");
              }          
            }
            else
            {
              NWNX.Events.SetEventResult("0");
              NWNX.Events.SkipEvent();

              if (NWNX.Events.GetCurrentEvent() == "NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE")
                player.SendMessage($"Le don maîtrise des extracteur de roche est requis pour pouvoir utiliser cet outil.");
            }
              break;
        }
      }
      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleBeforeItemAddedToRefinery(uint oidSelf)
    {
      NWItem oItem = NWNX.Object.StringToObject(NWNX.Events.GetEventData("ITEM")).AsItem();

      if (oItem.Tag != "ore" || oItem.Tag != "mineral")
      {
        NWNX.Events.SkipEvent();
        NWScript.SpeakString("Seul le minerai peut être raffiné dans la fonderie.");
      }
      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleItemAddedToRefinery(uint oidSelf)
    {
      if(NWScript.GetInventoryDisturbType() == DisturbType.Added)
      {
        NWItem item = NWScript.GetInventoryDisturbItem().AsItem();

        if (item.Tag == "ore")
        {
          PlayerSystem.Player player;
          if (Players.TryGetValue(NWScript.GetLastDisturbed(), out player))
          {
            string reprocessingData = $"{item.Name} : Efficacité raffinage -30 % (base fonderie)";
            
            int value;
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.Reprocessing)), out value))
              reprocessingData += $"\n x1.{3*value} (Raffinage)";

            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.ReprocessingEfficiency)), out value))
              reprocessingData += $"\n x1.{2 * value} (Raffinage efficace)";

            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.VeldsparReprocessing)), out value)) // TODO : Je ne sais pas trop comment lier le skill de spécialité de raffinage autrement qu'en dur avec un switch case Veldspar, alors ID = X, case Scordite ID = X2, etc
              reprocessingData += $"\n x1.{2 * value} (Raffinage {item.Name})";

            float connectionsLevel;
            if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.Connections)), out connectionsLevel))
              reprocessingData += $"\n x{1.00 - connectionsLevel / 100} (Raffinage {item.Name})";

            player.SendMessage(reprocessingData);
          }
        }
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleRefineryClose(uint oidSelf)
    {
      PlayerSystem.Player player;
      if (Players.TryGetValue(NWScript.GetLastClosedBy(), out player))
      {
        NWPlaceable fonderie = oidSelf.AsPlaceable();
        float reprocessingEfficiency = 0.3f;

        float value;
        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.Reprocessing)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 3 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.ReprocessingEfficiency)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

        if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.Connections)), out value))
          reprocessingEfficiency += reprocessingEfficiency + 1 * value / 100;


        foreach (NWItem ore in fonderie.InventoryItems)
        {
          if(ore.Tag == "ore")
          {
            NWItem mineral;

            switch (ore.Name)
            {
              case "Veldspar":
                if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.VeldsparReprocessing)), out value))
                  reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

                mineral = NWScript.CreateItemOnObject("mineral", player, ore.StackSize * 41 * (int)reprocessingEfficiency).AsItem();
                mineral.Name = "Tritanium";
                ore.Destroy();
                
                break;

              case "Scordite":
                if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", NWNX.Creature.GetHighestLevelOfFeat(player, (int)Feat.ScorditeReprocessing)), out value))
                  reprocessingEfficiency += reprocessingEfficiency + 2 * value / 100;

                mineral = NWScript.CreateItemOnObject("mineral", player, ore.StackSize * 23 * (int)reprocessingEfficiency).AsItem();
                mineral.Name = "Tritanium";

                mineral = NWScript.CreateItemOnObject("mineral", player, ore.StackSize * 11 * (int)reprocessingEfficiency).AsItem();
                mineral.Name = "Pyerite";

                ore.Destroy();

                break;
            }
          }
        }
        
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
