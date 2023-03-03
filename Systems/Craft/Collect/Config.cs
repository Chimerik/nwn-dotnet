using System.Collections.Generic;

using Anvil.API;

using static NWN.Systems.LootSystem.Lootable;

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
        if (craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasValue)
          craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
        else
          craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 1;

        if (craftedItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").HasValue)
          craftedItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value += 1;
        else
          craftedItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 1;
      }

      switch (craftedItem.BaseItem.ItemType)
      {
        case BaseItemType.Armor:
          return GetArmorProperties(craftedItem, grade);
        case BaseItemType.SmallShield:
          return GetSmallShieldProperties(grade);
        case BaseItemType.LargeShield:
          return GetLargeShieldProperties(grade);
        case BaseItemType.TowerShield:
          return GetTowerShieldProperties(grade);
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
        case BaseItemType.Boots:
          return GetArmorPartProperties();
        case BaseItemType.Gloves:
        case BaseItemType.Bracer:
          return GetGlovesProperties();
        case BaseItemType.Amulet:
          return GetAmuletProperties(grade);
        case BaseItemType.Ring:
          return GetRingProperties(grade);
        case BaseItemType.Belt:
          return GetBeltProperties(grade);
      }

      switch (ItemUtils.GetItemCategory(craftedItem.BaseItem.ItemType))
      {
        case ItemUtils.ItemCategory.CraftTool:
          return GetToolProperties(craftedItem, grade);
        case ItemUtils.ItemCategory.OneHandedMeleeWeapon:
          return GetOneHandedMeleeWeaponProperties();
        case ItemUtils.ItemCategory.TwoHandedMeleeWeapon:
          return GetTwoHandedMeleeWeaponProperties();
        case ItemUtils.ItemCategory.RangedWeapon:
          return GetRangedWeaponProperties();
        case ItemUtils.ItemCategory.Ammunition:
          return GetAmmunitionProperties();
      }

      SetWeaponDamage(craftedItem, grade - 1);

      Utils.LogMessageToDMs($"No craft property found for category {itemCategory} grade {grade}");

      return new List<ItemProperty>();
    }
    public static void SetWeaponDamage(NwItem craftedItem, int grade)
    {
      if (!ItemUtils.itemDamageDictionary.ContainsKey(craftedItem.BaseItem.ItemType))
        return;

      if(grade >  0) // cas des objets craftés. Les dégâts sont alors égaux à ceux de la config
      {
        craftedItem.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").Value = ItemUtils.itemDamageDictionary[craftedItem.BaseItem.ItemType][grade, 0];
        craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").Value = ItemUtils.itemDamageDictionary[craftedItem.BaseItem.ItemType][grade, 1];
      }
      else // cas des objets lootés. Les dégâts sont alors aléatoires. Mais ça ne va pas suffire, car il me faudra le niveau du loot
      {

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

          switch (craftedItem.BaseACValue)
          {
            case 1:
            case 2:
            case 3:
            case 4:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)14, craftedItem.BaseACValue * 5));
              break;
            case 5:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)14, 30));
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 5));
              break;
            case 6:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 10));
              break;
            case 7:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 15));
              break;
            case 8:
              badArmor.Add(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 20));
              break;
          }
          break;

        default:
          badArmor.Add(ItemProperty.ACBonus(7));
          break;
      }

      return badArmor;
    }
    public static List<ItemProperty> GetSmallShieldProperties(int materialTier)
    {
      List<ItemProperty> shield = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
        case 1:
          shield.Add(ItemProperty.ACBonus(2 + 2 * materialTier));
          shield.Add(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 5));
          break;
        default:
          shield.Add(ItemProperty.ACBonus(2));
          break;
      }

      return shield;
    }
    public static List<ItemProperty> GetLargeShieldProperties(int materialTier)
    {
      List<ItemProperty> shield = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
        case 1:
          shield.Add(ItemProperty.ACBonus(4 + 2 * materialTier));
          shield.Add(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 10));
          break;
        default:
          shield.Add(ItemProperty.ACBonus(2));
          break;
      }

      return shield;
    }
    public static List<ItemProperty> GetTowerShieldProperties(int materialTier)
    {
      List<ItemProperty> shield = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
        case 1:
          shield.Add(ItemProperty.ACBonus(2 * materialTier));
          shield.Add(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 20));
          break;
        default:
          shield.Add(ItemProperty.ACBonus(2));
          break;
      }

      return shield;
    }
    public static List<ItemProperty> GetToolProperties(NwItem craftedItem, int materialTier)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = 5 + 5 * materialTier;
      craftedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = 5 + 5 * materialTier;
      craftedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value = materialTier;

      List<ItemProperty> tool = new List<ItemProperty>();
      tool.Add(ItemProperty.Quality(IPQuality.Unknown));

      return tool;
    }
    public static List<ItemProperty> GetOneHandedMeleeWeaponProperties()
    {
      List<ItemProperty> oneHanded = new List<ItemProperty>();
      oneHanded.Add(ItemProperty.AttackBonus(2));

      return oneHanded;
    }
    public static List<ItemProperty> GetRangedWeaponProperties()
    {
      List<ItemProperty> oneHanded = new List<ItemProperty>();
      oneHanded.Add(ItemProperty.AttackBonus(2));

      return oneHanded;
    }
    public static List<ItemProperty> GetTwoHandedMeleeWeaponProperties()
    {
      List<ItemProperty> twoHanded = new List<ItemProperty>();
      twoHanded.Add(ItemProperty.AttackBonus(4));

      return twoHanded;
    }
    public static List<ItemProperty> GetArmorPartProperties()
    {
      List<ItemProperty> armorPart = new List<ItemProperty>();
      armorPart.Add(ItemProperty.ACBonus(7));

      return armorPart;
    }
    public static List<ItemProperty> GetGlovesProperties()
    {
      List<ItemProperty> gloves = new List<ItemProperty>();
      gloves.Add(ItemProperty.ACBonus(7));
      gloves.Add(ItemProperty.AttackBonus(2));

      return gloves;
    }
    public static List<ItemProperty> GetAmmunitionProperties()
    {
      List<ItemProperty> ammunition = new List<ItemProperty>();
      ammunition.Add(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1));

      return ammunition;
    }
    public static List<ItemProperty> GetRingProperties(int materialTier)
    {
      List<ItemProperty> ring = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
          ring.Add(ItemProperty.SkillBonus(Skill.Hide, 1));
          break;
        case 1:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fire, 1));
          break;
        case 2:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Cold, 1));
          ring.Add(ItemProperty.AbilityBonus(IPAbility.Dexterity, 1));
          ring.Add(ItemProperty.SkillBonus(Skill.MoveSilently, 1));
          break;
        case 3:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Acid, 1));
          ring.Add(ItemProperty.SkillBonus(Skill.Hide, 1));
          break;
        case 4:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Electrical, 1));
          ring.Add(ItemProperty.AbilityBonus(IPAbility.Dexterity, 1));
          ring.Add(ItemProperty.SkillBonus(Skill.MoveSilently, 1));
          break;
        case 5:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Negative, 1));
          ring.Add(ItemProperty.SkillBonus(Skill.Hide, 1));
          break;
        case 6:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Positive, 1));
          ring.Add(ItemProperty.AbilityBonus(IPAbility.Dexterity, 1));
          ring.Add(ItemProperty.SkillBonus(Skill.MoveSilently, 1));
          break;
        case 7:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Sonic, 1));
          ring.Add(ItemProperty.SkillBonus(Skill.Hide, 1));
          break;
        case 8:
          ring.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Divine, 1));
          ring.Add(ItemProperty.AbilityBonus(IPAbility.Dexterity, 1));
          break;
      }

      return ring;
    }
    public static List<ItemProperty> GetBeltProperties(int materialTier)
    {
      List<ItemProperty> belt = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
          belt.Add(ItemProperty.SkillBonus(Skill.Spot, 1));
          break;
        case 1:
          belt.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fire, 1));
          break;
        case 2:
          belt.Add(ItemProperty.AbilityBonus(IPAbility.Strength, 1));
          belt.Add(ItemProperty.SkillBonus(Skill.Listen, 1));
          break;
        case 3:
          belt.Add(ItemProperty.SkillBonus(Skill.Spot, 1));
          belt.Add(ItemProperty.SkillBonus(Skill.Discipline, 1));
          break;
        case 4:
          belt.Add(ItemProperty.AbilityBonus(IPAbility.Strength, 1));
          belt.Add(ItemProperty.SkillBonus(Skill.Listen, 1));
          break;
        case 5:
          belt.Add(ItemProperty.SkillBonus(Skill.Spot, 1));
          belt.Add(ItemProperty.SkillBonus(Skill.Discipline, 1));
          break;
        case 6:
          belt.Add(ItemProperty.AbilityBonus(IPAbility.Strength, 1));
          belt.Add(ItemProperty.SkillBonus(Skill.Listen, 1));
          break;
        case 7:
          belt.Add(ItemProperty.SkillBonus(Skill.Spot, 1));
          belt.Add(ItemProperty.SkillBonus(Skill.Discipline, 1));
          break;
        case 8:
          belt.Add(ItemProperty.AbilityBonus(IPAbility.Strength, 1));
          break;
      }

      return belt;
    }
    public static List<ItemProperty> GetAmuletProperties(int materialTier)
    {
      List<ItemProperty> amulet = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
          amulet.Add(ItemProperty.SkillBonus(Skill.MoveSilently, 1));
          break;
        case 1:
          amulet.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Poison, 1));
          break;
        case 2:
          amulet.Add(ItemProperty.AbilityBonus(IPAbility.Constitution, 1));
          amulet.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fear, 1));
          break;
        case 3:
          amulet.Add(ItemProperty.SkillBonus(Skill.Hide, 1));
          amulet.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Disease, 1));
          break;
        case 4:
          amulet.Add(ItemProperty.AbilityBonus(IPAbility.Constitution, 1));
          amulet.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.MindAffecting, 1));
          break;
        case 5:
          amulet.Add(ItemProperty.SkillBonus(Skill.Hide, 1));
          amulet.Add(ItemProperty.BonusSavingThrowVsX(IPSaveVs.Death, 1));
          break;
        case 6:
          amulet.Add(ItemProperty.AbilityBonus(IPAbility.Constitution, 1));
          amulet.Add(ItemProperty.BonusSavingThrow(IPSaveBaseType.Will, 1));
          break;
        case 7:
          amulet.Add(ItemProperty.BonusSavingThrow(IPSaveBaseType.Fortitude, 1));
          break;
        case 8:
          amulet.Add(ItemProperty.AbilityBonus(IPAbility.Constitution, 1));
          amulet.Add(ItemProperty.BonusSavingThrow(IPSaveBaseType.Will, 1));
          amulet.Add(ItemProperty.BonusSavingThrow(IPSaveBaseType.Fortitude, 1));
          amulet.Add(ItemProperty.BonusSavingThrow(IPSaveBaseType.Reflex, 1));
          break;
      }

      return amulet;
    }
  }
}
