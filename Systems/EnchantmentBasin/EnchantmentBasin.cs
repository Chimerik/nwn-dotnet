using System;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class EnchantmentBasinSystem
  {
    private class EnchantmentBasin
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
      private int maxDamageBonus;
      private int maxSavingThrowBonus;
      private int maxRegenBonus;

      public EnchantmentBasin(
        float costRate = 1.0f,
        int minSuccessPercent = 1,
        int maxSuccessPercent = 99,
        int maxCostRateForSuccessRateMin = 1000000,
        bool isAttackBonusEnabled = true,
        bool isACBonusEnabled = true,
        bool isAbilityBonusEnabled = true,
        bool isDamageBonusEnabled = true,
        bool isSavingThrowBonusEnabled = true,
        bool isRegenBonusEnabled = true,
        int maxAttackBonus = 20,
        int maxACBonus = 20,
        int maxAbilityBonus = 12,
        int maxDamageBonus = NWScript.DAMAGE_BONUS_20,
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

      public void DrawMenu(PlayerSystem.Player player, uint oItem)
      {
        player.menu.Clear();
        player.menu.title = "Choisissez un enchantement a appliquer sur votre objet";

        bool isWeapon = ItemUtils.IsWeapon(oItem);

        if (isAttackBonusEnabled && isWeapon) AddAttackBonusToMenu(player, oItem);
        if (isDamageBonusEnabled && isWeapon) AddDamageBonusToMenu(player, oItem);
        if (isACBonusEnabled) AddACBonusToMenu(player, oItem);
        if (isAbilityBonusEnabled) AddAbilityBonusToMenu(player, oItem);
        if (isSavingThrowBonusEnabled) AddSavingThrowBonusToMenu(player, oItem);
        if (isRegenBonusEnabled) AddRegenBonusToMenu(player, oItem);

        player.menu.Draw();
      }

      private void DrawCostPage(PlayerSystem.Player player, uint oItem, ItemProperty ip, string enchantmentName)
      {
        // Create a copy item to evaluate enchantment cost
        uint oCopy = NWScript.CopyItem(oItem, player.oid);

        // Enchant copy item to evaluate cost
        ItemPropertyUtils.ReplaceItemProperty(oCopy, ip);

        // Calculate cost
        int baseCost = ItemUtils.GetIdentifiedGoldPieceValue(oCopy) - ItemUtils.GetIdentifiedGoldPieceValue(oItem);
        int cost = (int)(baseCost * costRate);

        // Destroy the cloned item
        NWScript.DestroyObject(oCopy);

        // Calculate success rate
        int successPercent = (int)Utils.ScaleToRange(cost, maxCostRateForSuccessRateMin, 0, minSuccessPercent, maxSuccessPercent);
        successPercent = Math.Clamp(successPercent, minSuccessPercent, maxSuccessPercent);

        player.menu.Clear();
        player.menu.title = $"Desirez vous ajouter l'enchantement {enchantmentName} a l'item {NWScript.GetName(oItem)} ? " +
          $"Il vous en coutera {cost} POS. " +
          $"Vous avez {successPercent * 100}% de chance de reussite. " +
          $"En cas d'échec, l'item sera detruit de facon permanente.";
        player.menu.choices.Add(("Confirmer", () => HandleConfirm(player, oItem, ip, cost, successPercent)));
        player.menu.choices.Add(("Retour", () => DrawMenu(player, oItem)));
        player.menu.Draw();
      }

      private void HandleConfirm(PlayerSystem.Player player, uint oItem, ItemProperty ip, int cost, int successPercent)
      {
        if (NWScript.GetGold(player.oid) <= cost)
        {
          NWScript.SendMessageToPC(player.oid, "Vous n'avez pas la quantité d'or requise pour effectuer cet enchantement.");
          return;
        }

        NWScript.TakeGoldFromCreature(cost, player.oid, 1);

        var roll = Utils.random.Next(1, 100);
        if (roll <= successPercent)
        {
          // Success
          ItemPropertyUtils.ReplaceItemProperty(oItem, ip);
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_HOLY_AID), oItem);
        } else
        {
          // Failure
          NWScript.DestroyObject(oItem);
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_FLAME_M), oItem);
        }
      }

      private void AddAttackBonusToMenu(PlayerSystem.Player player, uint oItem)
      {
        var currentAttackBonus = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ATTACK_BONUS);
        if (currentAttackBonus < maxAttackBonus)
        {
          player.menu.choices.Add((
            "Bonus d'attaque",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyAttackBonus(currentAttackBonus + 1),
              $"Bonus d'attaque +{currentAttackBonus + 1}"
            )
          ));
        }
      }

      private void AddACBonusToMenu(PlayerSystem.Player player, uint oItem)
      {
        var currentAC = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_AC_BONUS);
        if (currentAC < maxACBonus)
        {
          player.menu.choices.Add((
            "Bonus d'armure",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyACBonus(currentAC + 1),
              $"Bonus d'armure +{currentAC + 1}"
            )
          ));
        }
      }

      private void AddAbilityBonusToMenu(PlayerSystem.Player player, uint oItem)
      {
        player.menu.choices.Add((
          "Bonus de caracteristique",
          () => DrawAbilityBonusPage(player, oItem)
        ));
      }

      private void DrawAbilityBonusPage(PlayerSystem.Player player, uint oItem)
      {
        player.menu.Clear();
        player.menu.title = "Choisissez une caracteristique";

        // STR
        var currentSTR = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ABILITY_BONUS, NWScript.ABILITY_STRENGTH);
        if (currentSTR < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Force",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyAbilityBonus(NWScript.ABILITY_STRENGTH, currentSTR + 1),
              $"Bonus de force +{currentSTR + 1}"
            )
          ));
        }

        // DEX
        var currentDEX = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ABILITY_BONUS, NWScript.ABILITY_DEXTERITY);
        if (currentDEX < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Dexterite",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyAbilityBonus(NWScript.ABILITY_DEXTERITY, currentDEX + 1),
              $"Bonus de dexterite +{currentDEX + 1}"
            )
          ));
        }

        // CON
        var currentCON = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ABILITY_BONUS, NWScript.ABILITY_CONSTITUTION);
        if (currentCON < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Constitution",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyAbilityBonus(NWScript.ABILITY_CONSTITUTION, currentCON + 1),
              $"Bonus de constitution +{currentCON + 1}"
            )
          ));
        }

        // INT
        var currentINT = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ABILITY_BONUS, NWScript.ABILITY_INTELLIGENCE);
        if (currentINT < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Intelligence",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyAbilityBonus(NWScript.ABILITY_INTELLIGENCE, currentINT + 1),
              $"Bonus d'intelligence +{currentINT + 1}"
            )
          ));
        }

        // CHA
        var currentCHA = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ABILITY_BONUS, NWScript.ABILITY_CHARISMA);
        if (currentCHA < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Charisme",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyAbilityBonus(NWScript.ABILITY_CHARISMA, currentCHA + 1),
              $"Bonus de charisme +{currentCHA + 1}"
            )
          ));
        }

        // WIS
        var currentWIS = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ABILITY_BONUS, NWScript.ABILITY_WISDOM);
        if (currentWIS < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Sagesse",
            () => DrawCostPage(
              player,
              oItem,
              NWScript.ItemPropertyAbilityBonus(NWScript.ABILITY_WISDOM, currentWIS + 1),
              $"Bonus de sagesse +{currentWIS + 1}"
            )
          ));
        }

        player.menu.choices.Add((
          "Retour",
          () => DrawMenu(player, oItem)
        ));
        player.menu.Draw();
      }

      private void AddDamageBonusToMenu(PlayerSystem.Player player, uint oItem)
      {
        // TODO
      }

      private void AddSavingThrowBonusToMenu(PlayerSystem.Player player, uint oItem)
      {
        // TODO
      }

      private void AddRegenBonusToMenu(PlayerSystem.Player player, uint oItem)
      {
        // TODO
      }
    }
  }
}
