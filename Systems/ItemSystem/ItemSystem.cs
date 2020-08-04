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

      switch (oidSelf.AsObject().Tag)
      {
        case "pccorpse":
          if (oItem.Tag == "item_pccorpse")
          {
            // TODO : détruire l'objet corps en BDD également where _PC_ID
            NWScript.DestroyObject(oidSelf);
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

      switch (oidSelf.AsObject().Tag)
      {
        case "pccorpse":
            // TODO : mettre à jour le cadavre serialisé en BDD
          break;
      }
      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
