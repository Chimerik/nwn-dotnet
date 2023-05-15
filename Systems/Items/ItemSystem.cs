using Anvil.API;
using NWN.Core.NWNX;
using Anvil.Services;
using System.Linq;
using Anvil.API.Events;
using NLog;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ItemSystem))]
  public partial class ItemSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static FeedbackService feedbackService;
    private readonly ScriptHandleFactory scriptHandleFactory;
    public static ScriptCallbackHandle removeCoreHandle;
    private static Effect corePotionEffect;
    public ItemSystem(FeedbackService feedback, ScriptHandleFactory scriptFactory)
    {
      feedbackService = feedback;
      scriptHandleFactory = scriptFactory;

      removeCoreHandle = scriptHandleFactory.CreateUniqueHandler(Potion.RemoveCore);

      corePotionEffect = Effect.RunAction(null, removeCoreHandle);
      corePotionEffect = Effect.LinkEffects(corePotionEffect, Effect.Icon(NwGameTables.EffectIconTable.GetRow(131)));
      corePotionEffect.Tag = "_CORE_EFFECT";
      corePotionEffect.SubType = EffectSubType.Supernatural;

      //NwModule.Instance.OnAcquireItem += OnAcquireItem;
      //NwModule.Instance.OnUnacquireItem += OnUnacquireItem;
    }
    public static void OnItemEquipBefore(OnItemEquip onItemEquip)
    {
      NwCreature oPC = onItemEquip.EquippedBy;
      NwItem oItem = onItemEquip.Item;

      if (oPC == null || oItem == null)
        return;

      if (onItemEquip.Slot == InventorySlot.LeftHand && !ItemUtils.CanBeEquippedInLeftHand(oItem.BaseItem.ItemType))
      {
        oPC.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(oItem.Name)} n'est pas une arme légère et ne peut être équipée dans la main gauche", ColorConstants.Red);
        onItemEquip.PreventEquip = true;
        return;
      }

      NwItem oUnequip = oPC.GetItemInSlot(onItemEquip.Slot);

      if (oUnequip != null && !oPC.Inventory.CheckFit(oUnequip))
      {
        oPC.ControllingPlayer.SendServerMessage($"Attention, votre inventaire est plein. Vous risqueriez de perdre votre {StringUtils.ToWhitecolor(oUnequip.Name)} en déséquipant !", ColorConstants.Red);
        onItemEquip.PreventEquip = true;
        return;
      }

      if(onItemEquip.Slot == InventorySlot.RightHand)
        oPC.BaseAttackCount = ItemUtils.GetWeaponAttackPerRound(oItem.BaseItem.ItemType);

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      player.SetMaxHP();
      player.endurance.additionnalMana = player.GetAdditionalMana();
    }

    public static void HandleUnequipItemBefore(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (!oPC.ControllingPlayer.IsValid || !oItem.IsValid || oPC.Inventory.CheckFit(oItem))
      {
        if (oPC.GetItemInSlot(InventorySlot.RightHand) is not null)
          oPC.BaseAttackCount = ItemUtils.GetWeaponAttackPerRound(oPC.GetItemInSlot(InventorySlot.RightHand).BaseItem.ItemType);
        else
          oPC.BaseAttackCount = 3;

        if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
          return;

        player.SetMaxHP();
        player.endurance.additionnalMana = player.GetAdditionalMana();

        return;
      }

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


      if (oItem.BaseItem.ItemType == BaseItemType.SpellScroll)
      {
        onItemUse.PreventUseItem = true;
        oPC.ControllingPlayer?.SendServerMessage("Les parchemins ne peuvent être utilisés qu'à but d'apprentissage", ColorConstants.Red);
        return;
      }

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player) || oItem == null)
        return;

      switch (oItem.Tag)
      {
        case "private_contract":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          if (oTarget != null && oTarget != player.oid.LoginCreature)
          {
            if (!player.windows.ContainsKey("resourceExchange")) player.windows.Add("resourceExchange", new PlayerSystem.Player.ResourceExchangeWindow(player, oTarget));
            else ((PlayerSystem.Player.ResourceExchangeWindow)player.windows["resourceExchange"]).CreateOwnerWindow(oTarget);
          }

          break;

        case "sequence_register":

          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          SequenceRegister.HandleCastSequence(player, oItem, oTarget);

          break;

        case "Peaudejoueur":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;
          oPC.RunEquip(onItemUse.Item, InventorySlot.CreatureSkin);
          break;

        case "potion_core_influx":
          Potion.CoreInflux(player, onItemUse.Item);
          break;

        case "potion_cure_mini":
          Potion.CureMini(oPC.ControllingPlayer);
          break;

        case "potion_cure_frog":
          Potion.CureFrog(oPC.ControllingPlayer);
          break;

        case "potion_alchimique":
          Potion.AlchemyEffect(onItemUse.Item, oPC.ControllingPlayer, oTarget);
          break;

        case "bank_contract":
          feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
          onItemUse.PreventUseItem = true;

          if (!player.windows.ContainsKey("bankContract")) player.windows.Add("bankContract", new PlayerSystem.Player.BankContractWindow(player, onItemUse.Item));
          else ((PlayerSystem.Player.BankContractWindow)player.windows["bankContract"]).CreateWindow();

          break;
      }

      if (oTarget is not null)
      { 
        switch (oTarget.Tag)
        {
          case "mineable_materia": // Utilisation en mode extraction

            if (!player.learnableSkills.ContainsKey(CustomSkill.MateriaExtraction))
            {
              player.oid.SendServerMessage("La base de la compétence d'extraction de dépot de matéria doit être apprise avant de pouvoir utiliser cet objet", ColorConstants.Red);
              return;
            }

            for (int i = 0; i < oItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            {
              if (oItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                continue;

              int inscriptionId = oItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

              if (inscriptionId >= CustomInscription.MateriaExtractionDurabilityMinor && inscriptionId <= CustomInscription.MateriaExtractionSpeedSupreme)
              {
                feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
                feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, oPC.ControllingPlayer);
                onItemUse.PreventUseItem = true;

                if (!player.windows.ContainsKey("materiaExtraction")) player.windows.Add("materiaExtraction", new PlayerSystem.Player.MateriaExtractionWindow(player, onItemUse.Item, oTarget));
                else ((PlayerSystem.Player.MateriaExtractionWindow)player.windows["materiaExtraction"]).CreateWindow(onItemUse.Item, oTarget);

                break;
              }
            }

            player.oid.SendServerMessage("Votre outil ne dispose d'aucune inscription permettant de procéder à une extraction de matéria. Pensez à faire appliquer une nouvelle inscription d'extraction !", ColorConstants.Red);
            return;

          case "forge":
          case "scierie":
          case "tannerie":

            for (int i = 0; i < oItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            {
              if (oItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                continue;

              int inscriptionId = oItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

              if (inscriptionId >= CustomInscription.MateriaProductionDurabilityMinor && inscriptionId <= CustomInscription.MateriaProductionQualitySupreme)
              {
                feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
                feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, oPC.ControllingPlayer);
                onItemUse.PreventUseItem = true;

                if (!player.windows.ContainsKey("craftWorkshop")) player.windows.Add("craftWorkshop", new PlayerSystem.Player.WorkshopWindow(player, oTarget.Tag, oItem));
                else ((PlayerSystem.Player.WorkshopWindow)player.windows["craftWorkshop"]).CreateWindow(oTarget.Tag, oItem);

                break;
              }
            }

            player.oid.SendServerMessage("Votre outil ne dispose d'aucune inscription permettant de manipuler de la matéria pour une production artisanale. Pensez à faire appliquer une nouvelle inscription de production !", ColorConstants.Red);
            return;
        }
      }
      else // Utilisation en mode scanner ?
      {
        for (int i = 0; i < oItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
        {
          if (oItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
            continue;

          int inscriptionId = oItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

          if (inscriptionId >= CustomInscription.MateriaDetectionDurabilityMinor && inscriptionId <= CustomInscription.MateriaExtractionDurabilitySupreme)
          {
            if (!player.learnableSkills.ContainsKey(CustomSkill.MateriaScanning))
            {
              player.oid.SendServerMessage("La base de la compétence de recherche de dépot de matéria doit être apprise avant de pouvoir utiliser cet objet", ColorConstants.Red);
              return;
            }

            feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
            feedbackService.AddFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, oPC.ControllingPlayer);
            onItemUse.PreventUseItem = true;

            if (!player.windows.ContainsKey("materiaDetector")) player.windows.Add("materiaDetector", new PlayerSystem.Player.MateriaDetectorWindow(player, onItemUse.Item));
            else ((PlayerSystem.Player.MateriaDetectorWindow)player.windows["materiaDetector"]).CreateWindow(onItemUse.Item);

            break;
          }
        }
      }

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));
      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, oPC.ControllingPlayer);
    }
    public static void OnAcquireItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;

      NwItem oItem = onAcquireItem.Item;
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;

      if (oPC.ControllingPlayer == null || oItem == null)
        return;

      if (oItem.Tag == "undroppable_item")
      {
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, oPC.ControllingPlayer);
        oItem.Clone(oAcquiredFrom);
        oItem.Destroy();
        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, oPC.ControllingPlayer);
        return;
      }

      if (oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasNothing && (oItem.BaseItem.EquipmentSlots != EquipmentSlots.None || oItem.BaseItem.ItemType == BaseItemType.CreatureItem))
      {
        int durability = ItemUtils.GetBaseItemCost(oItem) * 25;
        oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = durability;
        oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = durability;
      }

      if (oItem.Tag == "item_pccorpse" && oAcquiredFrom?.Tag == "pccorpse_bodybag")
      {
        PlayerSystem.DeletePlayerCorpseFromDatabase(oItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value);

        NwObject.FindObjectsWithTag<NwCreature>("pccorpse").Where(c => c.GetObjectVariable<LocalVariableInt>("_PC_ID").Value == oItem.GetObjectVariable<LocalVariableInt>("_PC_ID")).FirstOrDefault()?.Destroy();
        oAcquiredFrom.Destroy();
      }

      if (!oPC.LoginPlayer.IsDM && oItem.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").HasValue)
        LogUtils.LogMessage($"{oPC.Name} vient d'acquérir {oItem.Name} créé par {oItem.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").Value}", LogUtils.LogType.DMAction);

      if (oItem.BaseItem.IsStackable)
      {
        feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemReceived, oPC.ControllingPlayer);

        foreach (var inventoryItem in oPC.Inventory.Items)
        {
          bool sameItem = true;

          if (oItem != inventoryItem && inventoryItem.BaseItem.IsStackable && oItem.BaseItem.ItemType == inventoryItem.BaseItem.ItemType && oItem.Tag == inventoryItem.Tag && oItem.Name == inventoryItem.Name
            && oItem.StackSize + inventoryItem.StackSize <= oItem.BaseItem.MaxStackSize)
          {
            foreach (var localVar in oItem.LocalVariables)
            {
              switch (localVar)
              {
                case LocalVariableString stringVar:
                  if (stringVar.Value != inventoryItem.GetObjectVariable<LocalVariableString>(stringVar.Name).Value)
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

            inventoryItem.StackSize += oItem.StackSize;
            oItem.Destroy();
            break;
          }
        }

        feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemReceived, oPC.ControllingPlayer);
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
