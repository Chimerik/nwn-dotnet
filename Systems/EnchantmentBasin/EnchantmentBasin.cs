using System;
using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnchantmentBasinSystem
  {
    public class EnchantmentBasin
    {
      private float costRate;
      private int minSuccessPercent;
      private int maxSuccessPercent;
      private int maxCostRateForSuccessRateMin;

      // Enabled enchantment
      private bool isAttackBonusEnabled;
      private bool isACBonusEnabled;
      private bool isAbilityBonusEnabled;
      private bool isDamageBonusEnabled;
      private bool isSavingThrowBonusEnabled;
      private bool isRegenBonusEnabled;

      // maximum values
      private int maxAttackBonus;
      private int maxACBonus;
      private int maxAbilityBonus;
      private IPDamageBonus maxDamageBonus;
      private int maxSavingThrowBonus;
      private int maxRegenBonus;

      public PlayerSystem.Player player;
      public NwItem oItem;
      public NwPlaceable oContainer;

      public EnchantmentBasin(
        float costRate = 1.0f,
        int minSuccessPercent = 1,
        int maxSuccessPercent = 99,
        int maxCostRateForSuccessRateMin = 100000,
        bool isAttackBonusEnabled = true,
        bool isACBonusEnabled = true,
        bool isAbilityBonusEnabled = true,
        bool isDamageBonusEnabled = true,
        bool isSavingThrowBonusEnabled = true,
        bool isRegenBonusEnabled = true,
        int maxAttackBonus = 20,
        int maxACBonus = 20,
        int maxAbilityBonus = 12,
        IPDamageBonus maxDamageBonus = IPDamageBonus.Plus2d12,
        int maxSavingThrowBonus = 20,
        int maxRegenBonus = 20
      )
      {
        this.costRate = costRate;
        this.minSuccessPercent = minSuccessPercent;
        this.maxSuccessPercent = maxSuccessPercent;
        this.maxCostRateForSuccessRateMin = maxCostRateForSuccessRateMin;
        this.isAttackBonusEnabled = isAttackBonusEnabled;
        this.isACBonusEnabled = isACBonusEnabled;
        this.isAbilityBonusEnabled = isAbilityBonusEnabled;
        this.isDamageBonusEnabled = isDamageBonusEnabled;
        this.isSavingThrowBonusEnabled = isSavingThrowBonusEnabled;
        this.isRegenBonusEnabled = isRegenBonusEnabled;
        this.maxAttackBonus = maxAttackBonus;
        this.maxACBonus = maxACBonus;
        this.maxAbilityBonus = maxAbilityBonus;
        this.maxDamageBonus = maxDamageBonus;
        this.maxSavingThrowBonus = maxSavingThrowBonus;
        this.maxRegenBonus = maxRegenBonus;
      }

      public void DrawMenu(PlayerSystem.Player player, NwItem oItem, NwPlaceable oContainer)
      {
        this.player = player;
        this.oItem = oItem;
        this.oContainer = oContainer;

        DrawMenuPage();
      }

      private void DrawMenuPage()
      {
        player.menu.Clear();
        player.menu.titleLines.Add("Choisissez un enchantement a appliquer sur votre objet");

        if (isAttackBonusEnabled && ItemUtils.IsWeapon(oItem.BaseItem)) AddAttackBonusToMenu();
        if (isDamageBonusEnabled && ItemUtils.IsMeleeWeapon(oItem.BaseItem)) AddDamageBonusToMenu();
        if (isACBonusEnabled) AddACBonusToMenu();
        if (isAbilityBonusEnabled) AddAbilityBonusToMenu();
        if (isSavingThrowBonusEnabled) AddSavingThrowBonusToMenu();
        if (isRegenBonusEnabled) AddRegenBonusToMenu();

        player.menu.Draw();
      }

      private void DrawCostPage(ItemProperty ip, string enchantmentName)
      {
        // Create a copy item to evaluate enchantment cost
        NwItem oCopy = oItem.Clone(player.oid.ControlledCreature);

        // Enchant copy item to evaluate cost
        ItemPropertyUtils.ReplaceItemProperty(oCopy, ip);

        // Calculate cost
        int baseCost = ItemUtils.GetIdentifiedGoldPieceValue(oCopy) - ItemUtils.GetIdentifiedGoldPieceValue(oItem);
        int cost = (int)(baseCost * costRate);

        // Destroy the cloned item
        oCopy.Destroy();

        // Calculate success rate
        int successPercent = (int)Utils.ScaleToRange(cost, maxCostRateForSuccessRateMin, 0, minSuccessPercent, maxSuccessPercent);
        successPercent = Math.Clamp(successPercent, minSuccessPercent, maxSuccessPercent);

        player.menu.Clear();
        player.menu.titleLines = new List<string> {
          $"Desirez vous ajouter l'enchantement {enchantmentName} a l'item {oItem.Name} ? ",
          $"Il vous en coutera {cost} Pieces d'or. ",
          $"Vous avez {successPercent}% de chance de reussite. ",
          $"En cas d'échec, l'item sera detruit de facon permanente."
        };

        player.menu.choices.Add(("Confirmer", () => HandleConfirm(ip, cost, successPercent)));
        player.menu.choices.Add(("Retour", () => DrawMenuPage()));
        player.menu.Draw();
      }

      private void HandleConfirm(ItemProperty ip, int cost, int successPercent)
      {
        if (player.oid.LoginCreature.Gold <= cost)
        {
          player.oid.SendServerMessage("Vous n'avez pas la quantité d'or requise pour effectuer cet enchantement.");
          return;
        }

        player.oid.LoginCreature.TakeGold(cost);

        var roll = Utils.random.Next(1, 100);
        if (roll <= successPercent)
        {
          // Success
          ItemPropertyUtils.ReplaceItemProperty(oItem, ip);
          oContainer.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
        }
        else
        {
          // Failure
          oItem.Destroy();
          oContainer.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameM));
        }

        player.menu.Close();
      }

      private void AddAttackBonusToMenu()
      {
        var currentAttackBonus = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AttackBonus);
        if (currentAttackBonus < maxAttackBonus)
        {
          player.menu.choices.Add((
            "Bonus d'attaque",
            () => DrawCostPage(ItemProperty.AttackBonus(currentAttackBonus + 1),
              $"Bonus d'attaque +{currentAttackBonus + 1}"
            )
          ));
        }
      }

      private void AddACBonusToMenu()
      {
        var currentAC = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AcBonus);
        if (currentAC < maxACBonus)
        {
          player.menu.choices.Add((
            "Bonus d'armure",
            () => DrawCostPage(
              ItemProperty.ACBonus(currentAC + 1),
              $"Bonus d'armure +{currentAC + 1}"
            )
          ));
        }
      }

      private void AddAbilityBonusToMenu()
      {
        player.menu.choices.Add((
          "Bonus de caracteristique",
          () => DrawAbilityBonusPage()
        ));
      }

      private void DrawAbilityBonusPage()
      {
        player.menu.Clear();
        player.menu.titleLines.Add("Choisissez une caracteristique");

        AddAbilityBonusTypeToMenu(IPAbility.Strength);
        AddAbilityBonusTypeToMenu(IPAbility.Dexterity);
        AddAbilityBonusTypeToMenu(IPAbility.Constitution);
        AddAbilityBonusTypeToMenu(IPAbility.Intelligence);
        AddAbilityBonusTypeToMenu(IPAbility.Charisma);
        AddAbilityBonusTypeToMenu(IPAbility.Wisdom);

        player.menu.choices.Add((
          "Retour",
          () => DrawMenuPage()
        ));
        player.menu.Draw();
      }

      private void AddAbilityBonusTypeToMenu(IPAbility abilityBonusType)
      {
        var currentAbilityBonus = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AbilityBonus, (int)abilityBonusType);
        if (currentAbilityBonus < maxAbilityBonus)
        {
          var name = ItemPropertyUtils.AbilityBonusToString(abilityBonusType);
          ;
          player.menu.choices.Add((
            StringUtils.FirstCharToUpper(name),
            () => DrawCostPage(
              ItemProperty.AbilityBonus(abilityBonusType, currentAbilityBonus + 1),
              $"Bonus de {name} +{currentAbilityBonus + 1}"
              )
          ));
        }
      }

      private void AddDamageBonusToMenu()
      {
        player.menu.choices.Add((
          "Bonus de degats",
          () => DrawDamageBonusPage()
        ));
      }

      private void DrawDamageBonusPage()
      {
        player.menu.Clear();
        player.menu.titleLines.Add("Choisissez un type de degats");

        AddDamageBonusTypeToMenu(IPDamageType.Acid);
        AddDamageBonusTypeToMenu(IPDamageType.Cold);
        AddDamageBonusTypeToMenu(IPDamageType.Electrical);
        AddDamageBonusTypeToMenu(IPDamageType.Fire);
        AddDamageBonusTypeToMenu(IPDamageType.Sonic);
        AddDamageBonusTypeToMenu(IPDamageType.Bludgeoning);
        AddDamageBonusTypeToMenu(IPDamageType.Piercing);
        AddDamageBonusTypeToMenu(IPDamageType.Slashing);

        player.menu.choices.Add((
          "Retour",
          () => DrawMenuPage()
        ));
        player.menu.Draw();
      }

      private void AddDamageBonusTypeToMenu(IPDamageType damageBonusType)
      {
        IPDamageBonus nCurrentDamageBonus = (IPDamageBonus)ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.DamageBonus, (int)damageBonusType);

        if (ItemPropertyUtils.GetAverageDamageFromDamageBonus(nCurrentDamageBonus) < ItemPropertyUtils.GetAverageDamageFromDamageBonus(maxDamageBonus))
        {
          var nextDamageBonus = ItemPropertyUtils.GetNextAverageDamageBonus(nCurrentDamageBonus);
          var name = ItemPropertyUtils.DamageBonusTypeToString(damageBonusType);

          player.menu.choices.Add((
            StringUtils.FirstCharToUpper(name),
            () => DrawCostPage(ItemProperty.DamageBonus(damageBonusType, nextDamageBonus),
              $"Bonus de degats {name} +{ItemPropertyUtils.DamageBonusToString(nextDamageBonus)}"
            )
          ));
        }

      }

      private void AddSavingThrowBonusToMenu()
      {
        player.menu.choices.Add((
          "Bonus aux jets de sauvegarde",
          () => DrawSavingThrowBonusPage()
        ));
      }

      private void DrawSavingThrowBonusPage()
      {
        player.menu.Clear();
        player.menu.titleLines.Add("Choisissez un type de jet de sauvegarde");

        var currentUniversalSavingThrowBonus = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.SavingThrowBonus, (int)IPSaveVs.Universal);

        if (currentUniversalSavingThrowBonus < maxSavingThrowBonus)
        {
          player.menu.choices.Add((
            "Universel",
            () => DrawCostPage(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Universal, currentUniversalSavingThrowBonus + 1),
              $"Bonus de jet universel +{currentUniversalSavingThrowBonus + 1}"
            )
          ));
        }

        AddSavingThrowBonusTypeToMenu(IPSaveBaseType.Fortitude);
        AddSavingThrowBonusTypeToMenu(IPSaveBaseType.Will);
        AddSavingThrowBonusTypeToMenu(IPSaveBaseType.Reflex);

        player.menu.choices.Add((
          "Retour",
          () => DrawMenuPage()
        ));
        player.menu.Draw();
      }

      private void AddSavingThrowBonusTypeToMenu(IPSaveBaseType savingThrowBonusType)
      {
        var currentSavingThrowBonus = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.SavingThrowBonusSpecific, (int)savingThrowBonusType);

        if (currentSavingThrowBonus < maxSavingThrowBonus)
        {
          var name = ItemPropertyUtils.SavingThrowBonusToString(savingThrowBonusType);

          player.menu.choices.Add((
            StringUtils.FirstCharToUpper(name),
            () => DrawCostPage(ItemProperty.BonusSavingThrow(savingThrowBonusType, currentSavingThrowBonus + 1),
              $"Bonus de {name} +{currentSavingThrowBonus + 1}"
            )
          ));
        }
      }

      private void AddRegenBonusToMenu()
      {
        var currentRegenBonus = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.Regeneration);

        if (currentRegenBonus < maxRegenBonus)
        {
          player.menu.choices.Add((
            "Regeneration",
            () => DrawCostPage(ItemProperty.Regeneration(currentRegenBonus + 1),
              $"Bonus de regeneration +{currentRegenBonus + 1}"
            )
          ));
        }
      }
    }
  }
}
