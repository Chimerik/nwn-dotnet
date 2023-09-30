using Anvil.API;
using NWN.Core.NWNX;
using Anvil.Services;
using System.Linq;
using Anvil.API.Events;
using NLog;
using System;
using System.Collections.Generic;

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
    public static void OnItemEquipCheckArmorShieldProficiency(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null || !PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      switch(oItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
          if (!oPC.KnowsFeat(Feat.ShieldProficiency))
          {
            ApplyShieldArmorDisadvantageToPlayer(oPC);
            player.oid.SendServerMessage("Malus d'absence de maîtrise des boucliers appliqué !");
          }
          break;

        case BaseItemType.Armor:

          if (oItem.BaseACValue > 0 && oItem.BaseACValue < 3)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyLight))
            {
              ApplyShieldArmorDisadvantageToPlayer(oPC);
              player.oid.SendServerMessage("Malus d'absence de maîtrise des armures légères appliqué !");
            }

          if (oItem.BaseACValue > 2 && oItem.BaseACValue < 6)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyMedium))
            {
              ApplyShieldArmorDisadvantageToPlayer(oPC);
              player.oid.SendServerMessage("Malus d'absence de maîtrise des armures intermédiaires appliqué !");
            }

          if (oItem.BaseACValue > 6)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyHeavy))
            {
              ApplyShieldArmorDisadvantageToPlayer(oPC);
              player.oid.SendServerMessage("Malus d'absence de maîtrise des armures lourdes appliqué !");
            }

          break;
      }
    }
    //TODO : vérifier comment l'application se passe à la première connexion après reboot, puis aux connexions successives
    public static void OnItemUnEquipCheckArmorShieldProficiency(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null)
        return;

      switch (oItem.BaseItem.ItemType) // TODO : Il faudra penser à faire le check de suppression d'avantage si le perso apprend la maîtrise au cas où il a une armure ou bouclier d'équipé
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:

          if (!oPC.KnowsFeat(Feat.ShieldProficiency))
            RemoveShieldArmorDisadvantageToPlayer(oPC); 

          break;

        case BaseItemType.Armor:

          if (oItem.BaseACValue > 0 && oItem.BaseACValue < 3)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyLight))
              RemoveShieldArmorDisadvantageToPlayer(oPC);

          if (oItem.BaseACValue > 2 && oItem.BaseACValue < 6)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyMedium))
              RemoveShieldArmorDisadvantageToPlayer(oPC);

          if (oItem.BaseACValue > 6)
            if (!oPC.KnowsFeat(Feat.ArmorProficiencyHeavy))
              RemoveShieldArmorDisadvantageToPlayer(oPC);

          break;
      }
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
    public static void HandleUnacquirableItems(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;
      NwItem oItem = onAcquireItem.Item;

      if (oPC.ControllingPlayer == null || oItem == null || oItem.Tag != "undroppable_item")
        return;

      feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, oPC.ControllingPlayer);
      oItem.Clone(oAcquiredFrom);
      oItem.Destroy();
      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, oPC.ControllingPlayer);
    }
    public static void OnAcquireForceDurability(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;
      NwItem oItem = onAcquireItem.Item;

      if (oPC.ControllingPlayer == null || oItem == null)
        return;

      if (oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasNothing && (oItem.BaseItem.EquipmentSlots != EquipmentSlots.None || oItem.BaseItem.ItemType == BaseItemType.CreatureItem))
      {
        int durability = ItemUtils.GetBaseItemCost(oItem) * 25;
        oItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = durability;
        oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = durability;
      }
    }
    public static void OnAcquirePlayerCorpse(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;
      NwItem oItem = onAcquireItem.Item;

      if (oPC.ControllingPlayer == null || oItem == null)
        return;

      if (oItem.Tag == "item_pccorpse" && oAcquiredFrom?.Tag == "pccorpse_bodybag")
      {
        PlayerSystem.DeletePlayerCorpseFromDatabase(oItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value);

        NwObject.FindObjectsWithTag<NwCreature>("pccorpse").Where(c => c.GetObjectVariable<LocalVariableInt>("_PC_ID").Value == oItem.GetObjectVariable<LocalVariableInt>("_PC_ID")).FirstOrDefault()?.Destroy();
        oAcquiredFrom.Destroy();
      }
    }
    public static void OnAcquireDMCreatedItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;
      NwItem oItem = onAcquireItem.Item;

      if (oPC.ControllingPlayer == null || oItem == null || oPC.LoginPlayer.IsDM || oItem.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").HasNothing)
        return;

      LogUtils.LogMessage($"{oPC.Name} vient d'acquérir {oItem.Name} créé par {oItem.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").Value}", LogUtils.LogType.DMAction);
    }
    public static void MergeStackableItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;
      NwItem oItem = onAcquireItem.Item;

      if (oPC.ControllingPlayer == null || oItem == null || !oItem.BaseItem.IsStackable)
        return;

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
    public static void OnAcquireItemSavePlayer(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwCreature oPC = (NwCreature)onAcquireItem.AcquiredBy;
      NwItem oItem = onAcquireItem.Item;

      if (oPC.ControllingPlayer == null || oItem == null
        || !PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player) || player.pcState == PlayerSystem.Player.PcState.Offline)
        return;

      player.oid.ExportCharacter();

      //En pause jusqu'à ce que le système de transport soit en place
      //if (oPC.MovementRate != MovementRate.Immobile && oPC.TotalWeight > Encumbrance2da.encumbranceTable.GetDataEntry(oPC.GetAbilityScore(Ability.Strength)).heavy)
      //oPC.MovementRate = MovementRate.Immobile;        
    }
    public static void OnUnacquirePlayerCorpse(ModuleEvents.OnUnacquireItem onUnacquireItem)
    {
      NwItem oItem = onUnacquireItem.Item;

      if (onUnacquireItem.LostBy.ControllingPlayer == null || oItem == null || oItem.Tag != "item_pccorpse" || oItem.Possessor is not null) // signifie que l'item a été drop au sol et pas donné à un autre PJ ou mis dans un placeable
        return;

      NwCreature oCorpse = NwCreature.Deserialize(oItem.GetObjectVariable<LocalVariableString>("_SERIALIZED_CORPSE").Value.ToByteArray());
      oCorpse.Location = oItem.Location;
      Utils.DestroyInventory(oCorpse);
      oCorpse.AcquireItem(oItem);
      oCorpse.VisibilityOverride = VisibilityMode.Hidden;
      PlayerSystem.SetupPCCorpse(oCorpse);

      PlayerSystem.SavePlayerCorpseToDatabase(oItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value, oCorpse);
    }
    public static void OnUnacquireItemSavePlayer(ModuleEvents.OnUnacquireItem onUnacquireItem)
    {
      NwItem oItem = onUnacquireItem.Item;

      if (onUnacquireItem.LostBy.ControllingPlayer == null || oItem == null 
        || !PlayerSystem.Players.TryGetValue(onUnacquireItem.LostBy, out PlayerSystem.Player player) || player.pcState == PlayerSystem.Player.PcState.Offline )
        return;

      player.oid.ExportCharacter();

      //En pause jusqu'à ce que le système de transport soit en place
      //if (onUnacquireItem.LostBy.MovementRate == MovementRate.Immobile)
      //if (onUnacquireItem.LostBy.TotalWeight <= Encumbrance2da.encumbranceTable.GetDataEntry(onUnacquireItem.LostBy.GetAbilityScore(Ability.Strength)).heavy)
      //onUnacquireItem.LostBy.MovementRate = MovementRate.PC;
    }
    public static void NoEquipRuinedItem(OnItemValidateEquip onItemValidateEquip)
    {
      if (onItemValidateEquip.Item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue && onItemValidateEquip.Item.GetObjectVariable<LocalVariableInt>("_DURABILITY") < 1)
      {
        onItemValidateEquip.Result = EquipValidationResult.Denied;
        onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage($"{onItemValidateEquip.Item.Name} nécessite des réparations.", ColorConstants.Red);
      }
    }
    public static void NoUseRuinedItem(OnItemValidateUse onItemValidateUse)
    {
      if (onItemValidateUse.Item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue && onItemValidateUse.Item.GetObjectVariable<LocalVariableInt>("_DURABILITY") < 1)
        onItemValidateUse.CanUse = false;
    }
    public static void OnCheckProficiencies(OnCreatureCheckProficiencies onCheck)
    {
      if (onCheck.Item != null && onCheck.Item.BaseItem != null && onCheck.Item.BaseItem.EquipmentSlots != EquipmentSlots.None)
        onCheck.ResultOverride = CheckProficiencyOverride.HasProficiency;
    }
    public static void ApplyShieldArmorDisadvantageToPlayer(NwCreature playerCreature)
    {
      if (!PlayerSystem.Players.TryGetValue(playerCreature, out PlayerSystem.Player player))
        return;

      playerCreature.OnSpellAction += SpellSystem.NoArmorShieldProficiencyOnSpellInput;
      player.AddDisadvantage(new List<PlayerSystem.Advantage> { PlayerSystem.Advantage.SaveStrength, PlayerSystem.Advantage.SaveDexterity, PlayerSystem.Advantage.SaveConstitution, PlayerSystem.Advantage.SaveIntelligence, PlayerSystem.Advantage.SaveWisdom, PlayerSystem.Advantage.SaveCharisma, PlayerSystem.Advantage.SkillStrength, PlayerSystem.Advantage.SkillDexterity, PlayerSystem.Advantage.AttackStrength, PlayerSystem.Advantage.AttackDexterity });
    }
    public static void RemoveShieldArmorDisadvantageToPlayer(NwCreature playerCreature)
    {
      if (!PlayerSystem.Players.TryGetValue(playerCreature, out PlayerSystem.Player player))
        return;

      playerCreature.OnSpellAction -= SpellSystem.NoArmorShieldProficiencyOnSpellInput;
      player.AddAdvantage(new List<PlayerSystem.Advantage> { PlayerSystem.Advantage.SaveStrength, PlayerSystem.Advantage.SaveDexterity, PlayerSystem.Advantage.SaveConstitution, PlayerSystem.Advantage.SaveIntelligence, PlayerSystem.Advantage.SaveWisdom, PlayerSystem.Advantage.SaveCharisma, PlayerSystem.Advantage.SkillStrength, PlayerSystem.Advantage.SkillDexterity, PlayerSystem.Advantage.AttackStrength, PlayerSystem.Advantage.AttackDexterity });
    }
  }
}
