using System;

using Anvil.API;

namespace NWN.Systems.Craft.Collect
{
  public class Config
  {
    public static void SetCraftItemProperties(NwItem craftedItem, int grade)
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
        case BaseItemType.Armor: GetArmorProperties(craftedItem, grade); return;
        case BaseItemType.SmallShield: GetSmallShieldProperties(craftedItem, grade); return;
        case BaseItemType.LargeShield: GetLargeShieldProperties(craftedItem, grade); return;
        case BaseItemType.TowerShield: GetTowerShieldProperties(craftedItem, grade); return;
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
        case BaseItemType.Boots:
        case BaseItemType.Gloves:
        case BaseItemType.Bracer:
        case BaseItemType.Belt: GetArmorPartProperties(craftedItem); return;
      }

      SetWeaponDamage(craftedItem, grade - 1);
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

    public static void GetArmorProperties(NwItem craftedItem, int grade)
    {
      switch (grade)
      {
        case 0:
        case 1:

          craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = craftedItem.BaseACValue * 3 + 7 * grade;

          switch (craftedItem.BaseACValue)
          {
            case 1:
            case 2:
            case 3:
            case 4: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value = craftedItem.BaseACValue * 5; break;
            case 5: 
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value = 30;
              craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 5;
              break;
            case 6: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 10; break;
            case 7: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 15; break;
            case 8: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value = 20; break;
          }
          break;

        default: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 7; break;
      }
    }
    public static void GetSmallShieldProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value = 5;

      switch (materialTier)
      {
        case 0:
        case 1: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = 2 + 2 * materialTier; break;
        default: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 2; break;
      }
    }
    public static void GetLargeShieldProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value = 10;

      switch (materialTier)
      {
        case 0:
        case 1: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = 4 + 2 * materialTier; break;
        default: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 2; break;
      }
    }
    public static void GetTowerShieldProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value = 40;

      switch (materialTier)
      {
        case 0:
        case 1: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value = 2 * materialTier; break;
        default: craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 2; break;
      }
    }
    public static void GetArmorPartProperties(NwItem craftedItem) 
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value += 7; 
    }
  }
}
