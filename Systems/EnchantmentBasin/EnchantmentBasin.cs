using System;
using NWN.Enums;
using NWN.Enums.Item;
using NWN.Enums.VisualEffect;

namespace NWN.Systems
{
  public static partial class EnchantmentBasinSystem
  {
    private class EnchantmentBasin
    {
      private PlayerSystem.Player player;
      private uint oItem;

      private float costRate;
      private int minSuccessPercent;
      private int maxSuccessPercent;
      private int maxCostRateForSuccessRateMin;
      private int maxAttackBonus;
      private int maxACBonus;
      private int maxAbilityBonus;
      private string maxDamageBonus;
      private int maxSavingThrowBonus;
      private int maxRegenBonus;

      public EnchantmentBasin(
        PlayerSystem.Player player,
        uint oItem,
        float costRate = 1.0f,
        int minSuccessPercent = 1,
        int maxSuccessPercent = 99,
        int maxCostRateForSuccessRateMin = 1000000,
        int maxAttackBonus = 2,
        int maxACBonus = 20,
        int maxAbilityBonus = 12,
        string maxDamageBonus = "2d12",
        int maxSavingThrowBonus = 20,
        int maxRegenBonus = 20
      )
      {
        this.player = player;
        this.oItem = oItem;
        this.costRate = costRate;
        this.minSuccessPercent = minSuccessPercent;
        this.maxSuccessPercent = maxSuccessPercent;
        this.maxCostRateForSuccessRateMin = maxCostRateForSuccessRateMin;
        this.maxAttackBonus = maxAttackBonus;
        this.maxACBonus = maxACBonus;
        this.maxAbilityBonus = maxAbilityBonus;
        this.maxDamageBonus = maxDamageBonus;
        this.maxSavingThrowBonus = maxSavingThrowBonus;
        this.maxRegenBonus = maxRegenBonus;
      }

      public void DrawMenu()
      {
        player.menu.Clear();
        player.menu.title = "Menu du bassin d'enchantement";

        // AB
        var currentAttackBonus = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AttackBonus);
        if (currentAttackBonus < maxAttackBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus d'attaque",
            () => HandleDrawCost(
              NWScript.ItemPropertyAttackBonus(currentAttackBonus + 1),
              $"Bonus d'attaque +{currentAttackBonus + 1}"
            )
          ));
        }

        // AC
        var currentAC = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.ACBonus);
        if (currentAC < maxACBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus d'armure",
            () => HandleDrawCost(
              NWScript.ItemPropertyACBonus(currentAC+ 1),
              $"Bonus d'armure +{currentAC + 1}"
            )
          ));
        }

        // STR
        var currentSTR = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AbilityBonus, (int)Enums.Item.Property.Ability.Strength);
        if (currentSTR < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus de force",
            () => HandleDrawCost(
              NWScript.ItemPropertyAbilityBonus(Enums.Ability.Strength, currentAC + 1),
              $"Bonus de force +{currentSTR + 1}"
            )
          ));
        }

        // DEX
        var currentDEX = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AbilityBonus, (int)Enums.Item.Property.Ability.Dexterity);
        if (currentDEX < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus de dexterite",
            () => HandleDrawCost(
              NWScript.ItemPropertyAbilityBonus(Enums.Ability.Dexterity, currentDEX + 1),
              $"Bonus de dexterite +{currentDEX + 1}"
            )
          ));
        }

        // CON
        var currentCON = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AbilityBonus, (int)Enums.Item.Property.Ability.Constitution);
        if (currentCON < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus de constitution",
            () => HandleDrawCost(
              NWScript.ItemPropertyAbilityBonus(Enums.Ability.Constitution, currentCON + 1),
              $"Bonus de constitution +{currentCON + 1}"
            )
          ));
        }

        // INT
        var currentINT= ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AbilityBonus, (int)Enums.Item.Property.Ability.Intelligence);
        if (currentINT < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus d'intelligence",
            () => HandleDrawCost(
              NWScript.ItemPropertyAbilityBonus(Enums.Ability.Intelligence, currentINT + 1),
              $"Bonus d'intelligence +{currentINT + 1}"
            )
          ));
        }

        // CHA
        var currentCHA = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AbilityBonus, (int)Enums.Item.Property.Ability.Charisma);
        if (currentCHA < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus de charisme",
            () => HandleDrawCost(
              NWScript.ItemPropertyAbilityBonus(Enums.Ability.Charisma, currentCHA + 1),
              $"Bonus de charisme +{currentCHA + 1}"
            )
          ));
        }

        // WIS
        var currentWIS = ItemUtils.GetItemPropertyBonus(oItem, ItemPropertyType.AbilityBonus, (int)Enums.Item.Property.Ability.Wisdom);
        if (currentWIS < maxAbilityBonus)
        {
          player.menu.choices.Add((
            "Ajouter bonus de sagesse",
            () => HandleDrawCost(
              NWScript.ItemPropertyAbilityBonus(Enums.Ability.Wisdom, currentWIS + 1),
              $"Bonus de sagesse +{currentWIS + 1}"
            )
          ));
        }

        player.menu.Draw();
      }

      private void HandleDrawCost(ItemProperty ip, string enchantmentName)
      {
        // Create a copy item to evaluate enchantment cost
        uint oCopy = NWScript.CopyItem(oItem, player);

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
        player.menu.title = $"Desirez vous ajouter l'enchantement {enchantmentName} a l'item {NWScript.GetName(oItem)} ?" +
          $"Il vous en coutera {cost} POS." +
          $"Vous avez {successPercent * 100}% de chance de reussite." +
          $"En cas d'échec, l'item sera detruit de facon permanente.";
        player.menu.choices.Add(("Confirmer.", () => HandleConfirm(ip, cost, successPercent)));
        player.menu.choices.Add(("Retour.", () => DrawMenu()));
        player.menu.choices.Add(("Fermer.", () => player.menu.Close()));
        player.menu.Draw();
      }

      private void HandleConfirm(ItemProperty ip, int cost, int successPercent)
      {
        if (player.Gold <= cost)
        {
          player.SendMessage("Vous n'avez pas la quantité d'or requise pour effectuer cet enchantement.");
          return;
        }

        NWScript.TakeGoldFromCreature(cost, player.oid, true);

        var roll = Utils.random.Next(1, 100);
        if (roll <= successPercent)
        {
          // Success
          ItemPropertyUtils.ReplaceItemProperty(oItem, ip);
          NWScript.ApplyEffectToObject(DurationType.Instant, NWScript.EffectVisualEffect((VisualEffect)Impact.HolyAid), oItem);
        } else
        {
          // Failure
          NWScript.DestroyObject(oItem);
          NWScript.ApplyEffectToObject(DurationType.Instant, NWScript.EffectVisualEffect((VisualEffect)Impact.FlameMedium), oItem);
        }
      }
    }
  }
}
