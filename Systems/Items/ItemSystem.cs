using Anvil.API;
using NWN.Core.NWNX;
using Anvil.Services;
using System.Linq;
using Anvil.API.Events;
using NLog;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ItemSystem))]
  public class ItemSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static FeedbackService feedbackService;
    public ItemSystem(FeedbackService feedback)
    {
      feedbackService = feedback;
      //NwModule.Instance.OnAcquireItem += OnAcquireItem;
      //NwModule.Instance.OnUnacquireItem += OnUnacquireItem;
    }
    public static void OnItemEquipBefore(OnItemEquip onItemEquip)
    {
      NwCreature oPC = onItemEquip.EquippedBy;
      NwItem oItem = onItemEquip.Item;

      if (oPC == null || oItem == null)
        return;

      NwItem oUnequip = oPC.GetItemInSlot(onItemEquip.Slot);
      
      if (oUnequip != null && !oPC.Inventory.CheckFit(oUnequip))
      {
        oPC.ControllingPlayer.SendServerMessage($"Attention, votre inventaire est plein. Vous risqueriez de perdre votre {oUnequip.Name} en déséquipant !", ColorConstants.Red);
        onItemEquip.PreventEquip = true;
        return;
      }
    }

    public static void HandleUnequipItemBefore(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (!oPC.ControllingPlayer.IsValid || !oItem.IsValid || oPC.Inventory.CheckFit(oItem))
        return;

      if (oPC.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_NOARMOR").HasValue
        || oPC.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_NOWEAPON").HasValue
        || oPC.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_NOACCESSORY").HasValue
        || oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY") <= 0)
      {
        oPC.ControllingPlayer.SendServerMessage($"Attention, votre inventaire est plein. Votre {oItem.Name} a été déposé au sol !", ColorConstants.Red);
        oItem.Clone(oPC.Location);
        oItem.Destroy();
        oPC.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_NOARMOR").Delete();
        oPC.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_NOWEAPON").Delete();
        oPC.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_NOACCESSORY").Delete();
      }
      else
        oPC.ControllingPlayer.SendServerMessage($"Attention, votre inventaire est plein. Vous risqueriez de perdre votre {oItem.Name} en déséquipant !", ColorConstants.Red);

      EventsPlugin.SkipEvent();
    }
    public static async void OnItemUseBefore(OnItemUse onItemUse)
    {
      NwCreature oPC = onItemUse.UsedBy;

      NwItem oItem = onItemUse.Item;
      NwGameObject oTarget = onItemUse.TargetObject;

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player) || oItem == null)
        return;
      
      switch (oItem.Tag)
      {
        case "oreextractor":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          if (player.windows.ContainsKey("materiaExtraction"))
            ((PlayerSystem.Player.MateriaExtractionWindow)player.windows["materiaExtraction"]).CreateWindow(onItemUse.Item, oTarget);
          else
            player.windows.Add("materiaExtraction", new PlayerSystem.Player.MateriaExtractionWindow(player, onItemUse.Item, oTarget));
          
          break;
        case "private_contract":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          if (oTarget != null && oTarget != player.oid.LoginCreature)
          {
            if (player.windows.ContainsKey("resourceExchange"))
              ((PlayerSystem.Player.ResourceExchangeWindow)player.windows["resourceExchange"]).CreateOwnerWindow(oTarget);
            else
              player.windows.Add("resourceExchange", new PlayerSystem.Player.ResourceExchangeWindow(player, oTarget));
          }

          break;
        case "shop_clearance":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          new PlayerShop(oPC.ControllingPlayer.LoginCreature, oItem);

          break;
        case "auction_clearanc":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          new PlayerAuction(oPC.ControllingPlayer.LoginCreature, oItem);

          break;
        case "sequence_register":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          new SequenceRegister(player, oItem, oTarget);

          break;
        case "Peaudejoueur":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          oPC.RunEquip(onItemUse.Item, InventorySlot.CreatureSkin);         
          break;
        case "potion_cure_mini":
            new PotionCureMini(oPC.ControllingPlayer);
          break;
        case "potion_cure_frog":
          new PotionCureFrog(oPC.ControllingPlayer);
          break;
        case "potion_alchimique":
          new PotionAlchimisteEffect(onItemUse.Item, oPC.ControllingPlayer, oTarget);
          break;
        case "bank_contract":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          if (player.windows.ContainsKey("bankContract"))
            ((PlayerSystem.Player.BankContractWindow)player.windows["bankContract"]).CreateWindow();
          else
            player.windows.Add("bankContract", new PlayerSystem.Player.BankContractWindow(player, oItem));

          break;
        case "learning_book":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          if (player.windows.ContainsKey("learnable"))
            ((PlayerSystem.Player.LearnableWindow)player.windows["learnable"]).CreateWindow();
          else
            player.windows.Add("learnable", new PlayerSystem.Player.LearnableWindow(player));

          break;
        case "materia_detector":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

            if (player.windows.ContainsKey("materiaDetector"))
              ((PlayerSystem.Player.MateriaDetectorWindow)player.windows["materiaDetector"]).CreateWindow(onItemUse.Item);
            else
            player.windows.Add("materiaDetector", new PlayerSystem.Player.MateriaDetectorWindow(player, onItemUse.Item));

          break;
      }

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));
      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
    }
    public static void OnAcquireItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;

      NwItem oItem = onAcquireItem.Item;
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;

      if (oPC.ControllingPlayer == null || oItem == null)
        return;

      Task wait = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
   
        if (Config.env == Config.Env.Prod && oItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value != Config.itemKey)
        {
          oItem.Destroy();
          oPC.LoginPlayer.SendServerMessage($"{oItem.Name.ColorString(ColorConstants.White)} est un objet invalide et n'aurait jamais du pouvoir être ramassé. Il a donc été détruit.");
          Utils.LogMessageToDMs($"{oPC.Name} ({oPC.LoginPlayer.PlayerName}) a tenté de ramassé l'objet invalide : {oItem.Name}");
        }
      });

      if (oItem.Tag == "undroppable_item")
      {
        oItem.Clone(oAcquiredFrom);
        oItem.Destroy();
        return;
      }

      if (oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasNothing && oItem.BaseItem.EquipmentSlots != EquipmentSlots.None)
      {
        int durability = ItemUtils.GetBaseItemCost(oItem) * 25;
        oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = durability;
        oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = durability;
      }

      if (oItem.Tag == "item_pccorpse" && oAcquiredFrom?.Tag == "pccorpse_bodybag")
      {
        PlayerSystem.DeletePlayerCorpseFromDatabase(oItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value);

        NwCreature oCorpse = NwObject.FindObjectsWithTag<NwCreature>("pccorpse").Where(c => c.GetObjectVariable<LocalVariableInt>("_PC_ID").Value == oItem.GetObjectVariable<LocalVariableInt>("_PC_ID")).FirstOrDefault();
        if(oCorpse != null)
          oCorpse.Destroy();
        oAcquiredFrom.Destroy();
      }

      if(oItem.BaseItem.IsStackable)
      {
        foreach(var inventoryItem in oPC.Inventory.Items)
        {
          bool sameItem = true;

          if(oItem != inventoryItem && inventoryItem.BaseItem.IsStackable && oItem.BaseItem.ItemType == inventoryItem.BaseItem.ItemType && oItem.Tag == inventoryItem.Tag && oItem.Name == inventoryItem.Name
            && oItem.StackSize + inventoryItem.StackSize <= oItem.BaseItem.MaxStackSize )
          {
            foreach(var localVar in oItem.LocalVariables)
            {
              switch (localVar)
              {
                case LocalVariableString stringVar:
                  if(stringVar.Value != inventoryItem.GetObjectVariable<LocalVariableString>(stringVar.Name).Value)
                    sameItem = false;
                  break;
                case LocalVariableInt intVar:
                  if (intVar.Value != inventoryItem.GetObjectVariable<LocalVariableInt>(intVar.Name).Value)
                    sameItem = false;
                  break;
                case LocalVariableFloat floatVar:
                  if (floatVar.Value != inventoryItem.GetObjectVariable<LocalVariableFloat>(floatVar.Name).Value)
                    sameItem = false;
                  break;
                case DateTimeLocalVariable dateVar:
                  if (dateVar.Value != inventoryItem.GetObjectVariable<DateTimeLocalVariable>(dateVar.Name).Value)
                    sameItem = false;
                  break;
              }

              if (!sameItem)
                break;
            }

            if (!sameItem)
              continue;

            Log.Info($"inventory item : {inventoryItem.Name} x {inventoryItem.StackSize}");
            inventoryItem.StackSize += oItem.StackSize;
            Log.Info($"inventory item : x {inventoryItem.StackSize}");
            oItem.Destroy();
            break;
          }
        }
      }

      //En pause jusqu'à ce que le système de transport soit en place
      //if (oPC.MovementRate != MovementRate.Immobile && oPC.TotalWeight > Encumbrance2da.encumbranceTable.GetDataEntry(oPC.GetAbilityScore(Ability.Strength)).heavy)
      //oPC.MovementRate = MovementRate.Immobile;

      if (PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player) && player.pcState != PlayerSystem.Player.PcState.Offline)
        player.oid.ExportCharacter();
    }
    public static void OnUnacquireItem(ModuleEvents.OnUnacquireItem onUnacquireItem)
    {
      NwItem oItem = onUnacquireItem.Item;

      if (onUnacquireItem.LostBy.ControllingPlayer == null || oItem == null)
        return;

      NwGameObject oGivenTo = oItem.Possessor;

      if (oItem.Tag == "item_pccorpse" && oGivenTo == null) // signifie que l'item a été drop au sol et pas donné à un autre PJ ou mis dans un placeable
      {
        NwCreature oCorpse = NwCreature.Deserialize(oItem.GetObjectVariable<LocalVariableString>("_SERIALIZED_CORPSE").Value.ToByteArray());
        oCorpse.Location = oItem.Location;
        Utils.DestroyInventory(oCorpse);
        oCorpse.AcquireItem(oItem);
        oCorpse.VisibilityOverride = VisibilityMode.Hidden;
        PlayerSystem.SetupPCCorpse(oCorpse);

        PlayerSystem.SavePlayerCorpseToDatabase(oItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value, oCorpse);
      }

      //En pause jusqu'à ce que le système de transport soit en place
      //if (onUnacquireItem.LostBy.MovementRate == MovementRate.Immobile)
      //if (onUnacquireItem.LostBy.TotalWeight <= Encumbrance2da.encumbranceTable.GetDataEntry(onUnacquireItem.LostBy.GetAbilityScore(Ability.Strength)).heavy)
      //onUnacquireItem.LostBy.MovementRate = MovementRate.PC;

      if (PlayerSystem.Players.TryGetValue(onUnacquireItem.LostBy, out PlayerSystem.Player player) && player.pcState != PlayerSystem.Player.PcState.Offline)
        player.oid.ExportCharacter();
    }
    public static void NoEquipRuinedItem(OnItemValidateEquip onItemValidateEquip)
    {
      if (onItemValidateEquip.Item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue && onItemValidateEquip.Item.GetObjectVariable<LocalVariableInt>("_DURABILITY") <= 0)
      {
        onItemValidateEquip.Result = EquipValidationResult.Denied;
        onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage($"{onItemValidateEquip.Item.Name} nécessite des réparations.", ColorConstants.Red);
      }
    }
    public static void NoUseRuinedItem(OnItemValidateUse onItemValidateUse)
    {
      if (onItemValidateUse.Item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue && onItemValidateUse.Item.GetObjectVariable<LocalVariableInt>("_DURABILITY") <= 0)
        onItemValidateUse.CanUse = false;
    }
  }
}
