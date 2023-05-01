using System;
using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems.Craft.Collect
{
  public class Config
  {
    public static List<ItemProperty> GetCraftItemProperties(NwItem craftedItem, int grade)
    {
      ItemUtils.ItemCategory itemCategory = ItemUtils.GetItemCategory(craftedItem.BaseItem.ItemType);

      craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = ItemUtils.GetBaseItemCost(craftedItem) * 100 * grade;
      craftedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = ItemUtils.GetBaseItemCost(craftedItem) * 100 * grade;

      if (grade == 0)
        craftedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value /= 2;
      else
      {
        if((itemCategory == ItemUtils.ItemCategory.TwoHandedMeleeWeapon || itemCategory == ItemUtils.ItemCategory.RangedWeapon) && grade % 2 == 0)
        {
          craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 2;
          craftedItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value += 2;
        }
        else
        {
          craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          craftedItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value += 1;
        }
      }

      switch (craftedItem.BaseItem.ItemType)
      {
        case BaseItemType.Armor: return GetArmorProperties(craftedItem, grade);
        case BaseItemType.SmallShield: return GetSmallShieldProperties(craftedItem, grade);
        case BaseItemType.LargeShield: return GetLargeShieldProperties(craftedItem, grade);
        case BaseItemType.TowerShield: return GetTowerShieldProperties(craftedItem, grade);
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
        case BaseItemType.Boots:
        case BaseItemType.Gloves:
        case BaseItemType.Bracer:
        case BaseItemType.Belt: return GetArmorPartProperties(craftedItem);
      }

      switch (itemCategory)
      {
        case ItemUtils.ItemCategory.CraftTool:
          return GetToolProperties(craftedItem, grade);
      }

      SetWeaponDamage(craftedItem, grade - 1);

      Utils.LogMessageToDMs($"No craft property found for category {itemCategory} grade {grade}");

      return new List<ItemProperty>();
    }
    public static void SetWeaponDamage(NwItem craftedItem, int grade)
    {
      if (!ItemUtils.itemDamageDictionary.ContainsKey(craftedItem.BaseItem.ItemType))
        return;

      if (grade >  0) // cas des objets craftés. Les dégâts sont alors égaux à ceux de la config
      {
        craftedItem.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").Value = ItemUtils.itemDamageDictionary[craftedItem.BaseItem.ItemType][grade, 0];
        craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").Value = ItemUtils.itemDamageDictionary[craftedItem.BaseItem.ItemType][grade, 1];
      }
      else // cas des objets lootés. Les dégâts sont alors aléatoires
      {
        grade = Math.Abs(grade);

        int minDamage = ItemUtils.itemDamageDictionary[craftedItem.BaseItem.ItemType][grade, 0];
        int maxDamage = ItemUtils.itemDamageDictionary[craftedItem.BaseItem.ItemType][grade, 1];

        int randMinDamage = Utils.random.Next((int)(minDamage * 0.5), minDamage + 1);
        int randMaxDamage = Utils.random.Next(randMinDamage, maxDamage + 1);

        craftedItem.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").Value = randMinDamage;
        craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").Value = randMaxDamage;
      }
    }

    public static List<ItemProperty> GetArmorProperties(NwItem craftedItem, int grade)
    {
      List<ItemProperty> badArmor = new List<ItemProperty>();

      switch (grade)
      {
        case 0:
        case 1:
          badArmor.Add(ItemProperty.ACBonus(craftedItem.BaseACValue * 3 + 7 * grade));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = craftedItem.BaseACValue * 3 + 7 * grade;

          switch (craftedItem.BaseACValue)
          {
            case 1:
            case 2:
            case 3:
            case 4:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)14, craftedItem.BaseACValue * 5));
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value = craftedItem.BaseACValue * 5;
              break;
            case 5:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)14, 30));
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value = 30;
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 5));
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 5;
              break;
            case 6:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 10));
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 10;
              break;
            case 7:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 15));
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 15;
              break;
            case 8:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 20));
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 20;
              break;
          }
          break;

        default:
          badArmor.Add(ItemProperty.ACBonus(7));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 7;
          break;
      }

      return badArmor;
    }
    public static List<ItemProperty> GetSmallShieldProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value = 5;

      List<ItemProperty> shield = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
        case 1:
          shield.Add(ItemProperty.ACBonus(2 + 2 * materialTier));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = 2 + 2 * materialTier;
          shield.Add(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 5));
          break;
        default:
          shield.Add(ItemProperty.ACBonus(2));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 2;
          break;
      }

      return shield;
    }
    public static List<ItemProperty> GetLargeShieldProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value = 10;

      List<ItemProperty> shield = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
        case 1:
          shield.Add(ItemProperty.ACBonus(4 + 2 * materialTier));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = 4 + 2 * materialTier;
          shield.Add(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 10));
          break;
        default:
          shield.Add(ItemProperty.ACBonus(2));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 2;
          break;
      }

      return shield;
    }
    public static List<ItemProperty> GetTowerShieldProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value = 40;

      List<ItemProperty> shield = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
        case 1:
          shield.Add(ItemProperty.ACBonus(2 * materialTier));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = 2 * materialTier;
          shield.Add(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 40));
          break;
        default:
          shield.Add(ItemProperty.ACBonus(2));
          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 2;
          break;
      }

      return shield;
    }
    public static List<ItemProperty> GetToolProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = 5 + 5 * materialTier;
      craftedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 5 + 5 * materialTier;
      craftedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value = materialTier;

      return new List<ItemProperty> { ItemProperty.Quality(IPQuality.Unknown) };
    }
    public static List<ItemProperty> GetArmorPartProperties(NwItem craftedItem) 
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 7;
      return new List<ItemProperty> { ItemProperty.ACBonus(7) }; 
    }
  }
}
