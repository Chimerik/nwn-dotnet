using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static class ItemUtils
  {
    public enum ItemCategory
    {
      Invalid = -1,
      OneHandedMeleeWeapon = 0,
      TwoHandedMeleeWeapon = 1,
      RangedWeapon = 2,
      Shield = 3,
      Armor = 4,
      Ammunition = 5,
      CraftTool = 6,
      Potions = 7,
      Scroll = 8,
      Clothes = 9,
    }

    public static ItemCategory GetItemCategory(BaseItemType baseItemType)
    {
      switch (baseItemType)
      {
        case BaseItemType.Armor:
        case BaseItemType.Helmet:
          return ItemCategory.Armor;
        case BaseItemType.SmallShield:
        case BaseItemType.TowerShield:
        case BaseItemType.LargeShield:
          return ItemCategory.Shield;
        case BaseItemType.Doubleaxe:
        case BaseItemType.Greataxe:
        case BaseItemType.Greatsword:
        case BaseItemType.Halberd:
        case BaseItemType.HeavyFlail:
        case BaseItemType.Quarterstaff:
        case BaseItemType.Scythe:
        case BaseItemType.TwoBladedSword:
        case BaseItemType.DireMace:
        case BaseItemType.Trident:
        case BaseItemType.ShortSpear:
          return ItemCategory.TwoHandedMeleeWeapon;
        case BaseItemType.Bastardsword:
        case BaseItemType.Longsword:
        case BaseItemType.Battleaxe:
        case BaseItemType.Club:
        case BaseItemType.Dagger:
        case BaseItemType.DwarvenWaraxe:
        case BaseItemType.Handaxe:
        case BaseItemType.Kama:
        case BaseItemType.Katana:
        case BaseItemType.Kukri:
        case BaseItemType.LightFlail:
        case BaseItemType.LightHammer:
        case BaseItemType.LightMace:
        case BaseItemType.Morningstar:
        case BaseItemType.Rapier:
        case BaseItemType.Shortsword:
        case BaseItemType.Scimitar:
        case BaseItemType.Sickle:
        case BaseItemType.Warhammer:
        case BaseItemType.Whip:
          return ItemCategory.OneHandedMeleeWeapon;
        case BaseItemType.HeavyCrossbow:
        case BaseItemType.LightCrossbow:
        case BaseItemType.Shortbow:
        case BaseItemType.Longbow:
        case BaseItemType.Dart:
        case BaseItemType.Sling:
        case BaseItemType.ThrowingAxe:
          return ItemCategory.RangedWeapon;
        case BaseItemType.Arrow:
        case BaseItemType.Bolt:
        case BaseItemType.Bullet:
          return ItemCategory.Ammunition;
        case BaseItemType.Potions:
        case BaseItemType.BlankPotion:
        case BaseItemType.EnchantedPotion:
          return ItemCategory.Potions;
        case BaseItemType.Scroll:
        case BaseItemType.BlankScroll:
        case BaseItemType.EnchantedScroll:
        case BaseItemType.SpellScroll:
          return ItemCategory.Scroll;
        case BaseItemType.Belt:
        case BaseItemType.Boots:
        case BaseItemType.Bracer:
        case BaseItemType.Cloak:
        case BaseItemType.Gloves:
          return ItemCategory.Clothes;
        case (BaseItemType)114: //marteau de forgeron
        case (BaseItemType)115: //extracteur de minerai
          return ItemCategory.CraftTool;
        default:
          return ItemCategory.Invalid;
      }
    }
    // ----------------------------------------------------------------------------
    // Removes all itemproperties with matching nItemPropertyType and
    // nItemPropertyDuration (a DURATION_TYPE_* constant)
    // ----------------------------------------------------------------------------
    public static void RemoveMatchingItemProperties(NwItem oItem, ItemPropertyType nItemPropertyType, EffectDuration nItemPropertyDuration = EffectDuration.Temporary, int nItemPropertySubType = -1)
    {
      foreach(ItemProperty ip in oItem.ItemProperties.Where(i => i.PropertyType == nItemPropertyType && (i.DurationType == nItemPropertyDuration || nItemPropertyDuration == (EffectDuration)(-1)) && (i.SubType == nItemPropertySubType || nItemPropertySubType == -1)))
        oItem.RemoveItemProperty(ip);
    }
    public static void DecreaseItemDurability(NwItem oItem)
    {
      if (oItem == null)
        return;

      int itemDurability = oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value; 
      if (itemDurability <= 1)
        oItem.Destroy();
      else
        oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value -= 1;
    }
    public static int GetIdentifiedGoldPieceValue(NwItem oItem)
    {
      bool isIdentified = oItem.Identified;
      if (isIdentified) oItem.Identified = true;
        int nGP = oItem.GoldValue;

      // Re-set the identification flag to its original if it has been changed.
      if (isIdentified) oItem.Identified = false;
      return nGP;
    }

    public static int GetItemPropertyBonus(NwItem oItem, ItemPropertyType ipType, int ipSubType = -1)
    {
      List<ItemProperty> sortedIP;

      if (ipSubType > -1)
        sortedIP = oItem.ItemProperties.Where(i => i.PropertyType == ipType && i.SubType == ipSubType).ToList();
      else
        sortedIP = oItem.ItemProperties.Where(i => i.PropertyType == ipType).ToList();

      ItemProperty maxIp = sortedIP.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
      int nPropBonus = 0;

      if (maxIp != null)
        nPropBonus = maxIp.CostTableValue;

      return nPropBonus;
    }
    public static int GetBaseItemCost(NwItem item)
    {
      float baseCost = 9999999;

      if (item.BaseItem.ItemType == BaseItemType.Armor)
        baseCost = Armor2da.armorTable.GetDataEntry(item.BaseACValue).cost;
      else
        baseCost = item.BaseItem.BaseCost;

      if (baseCost <= 0)
      {
        Utils.LogMessageToDMs($"{item.Name} - baseCost introuvable pour baseItemType : {item.BaseItem.ItemType}");
        return 999999;
      }

      return (int)baseCost;
    }
    public static string GetItemDurabilityState(NwItem item)
    {
      int durabilityState = item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value / item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 100;

      if (durabilityState == 100)
        return "Flambant neuf".ColorString(new Color(32, 255, 32));
      else if (durabilityState < 100 && durabilityState >= 75)
        return "Très bon état".ColorString(ColorConstants.Green);
      else if (durabilityState < 75 && durabilityState >= 50)
        return "Bon état".ColorString(ColorConstants.Red);
      else if (durabilityState < 50 && durabilityState >= 25)
        return "Usé".ColorString(ColorConstants.Lime);
      else if (durabilityState < 25 && durabilityState >= 5)
        return "Abimé".ColorString(ColorConstants.Orange);
      else if (durabilityState < 5 && durabilityState >= 1)
        return "Vétuste".ColorString(ColorConstants.Red);
      else if (durabilityState < 1)
        return "Ruiné".ColorString(ColorConstants.Red);

      return "";
    }
    public static DamageType GetDamageTypeFromItemProperty(IPDamageType ipDamageType)
    {
      switch(ipDamageType)
      {
        case IPDamageType.Bludgeoning: return DamageType.Bludgeoning;
        case IPDamageType.Piercing: return DamageType.Piercing;
        case IPDamageType.Slashing: return DamageType.Slashing;
        case IPDamageType.Acid: return DamageType.Acid;
        case IPDamageType.Magical: return DamageType.Magical;
        case IPDamageType.Fire: return DamageType.Fire;
        case IPDamageType.Cold: return DamageType.Cold;
        case IPDamageType.Electrical: return DamageType.Electrical;
        case IPDamageType.Divine: return DamageType.Divine;
        case IPDamageType.Negative: return DamageType.Negative;
        case IPDamageType.Positive: return DamageType.Positive;
        case IPDamageType.Sonic: return DamageType.Sonic;
        case IPDamageType.Physical: return (DamageType)8192; // Physical
        case (IPDamageType)14: return (DamageType)16384; // Elemental
        default: return DamageType.Slashing;
      }
    }
    public static int GetMaxDamage(NwBaseItem baseItem, NwCreature oCreature, bool IsRangedAttack)
    {
      int additionnalDamage = IsRangedAttack ? oCreature.GetAbilityModifier(Ability.Dexterity) : oCreature.GetAbilityModifier(Ability.Strength);
      return (baseItem.DieToRoll * baseItem.NumDamageDice) + additionnalDamage;
    }
    public static bool IsWeapon(NwBaseItem baseItem)
    {
      return baseItem.NumDamageDice > 0;
    }
    public static bool IsMeleeWeapon(NwBaseItem baseItem)
    {
      return baseItem.NumDamageDice > 0 && !baseItem.IsRangedWeapon;
    }
    public static bool IsTwoHandedWeapon(NwBaseItem baseItem, CreatureSize creatureSize)
    {
      return baseItem.NumDamageDice > 0 && baseItem.WeaponSize != BaseItemWeaponSize.Unknown && baseItem.WeaponSize > (BaseItemWeaponSize)creatureSize;
    }
    public static byte[] GetDamageDices(NwBaseItem baseItem)
    {
      return new byte[] { baseItem.DieToRoll, baseItem.NumDamageDice };
    }
    public static NwItem DeserializeAndAcquireItem(string itemTemplate, NwCreature receiver)
    {
      NwItem deserializedItem = NwItem.Deserialize(itemTemplate.ToByteArray());
      deserializedItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
      receiver.AcquireItem(deserializedItem);
      return deserializedItem;
    }
    public static void CreateShopSkillBook(NwItem skillBook, int featId)
    {
      skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
      skillBook.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value = featId;
      skillBook.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;

      try
      {
        Learnable learnable = SkillSystem.learnableDictionary[featId];
        skillBook.Name = $"Livre de compétence : {learnable.name}";
        skillBook.Description = learnable.description;
        skillBook.BaseGoldValue = (uint)(learnable.multiplier * 1000);
      }
      catch(Exception)
      {
        Utils.LogMessageToDMs($"ERROR - Could not find {featId} in the learnable dictionnary.");
        skillBook.Destroy();
      }
    }
    public static void CreateShopBlueprint(NwItem oBlueprint, int baseItemType)
    {
      Craft.Blueprint blueprint = new Craft.Blueprint(baseItemType);

      if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
        Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

      oBlueprint.Name = $"Patron original : {blueprint.name}";

      oBlueprint.BaseGoldValue = (uint)(blueprint.goldCost * 10);
      oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value = baseItemType;
      oBlueprint.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
    }
    public static string DisplayDamageType(DamageType damageType)
    {
      switch(damageType)
      {
        case DamageType.Bludgeoning:
          return "Contondant";
        case DamageType.Piercing:
          return "Perçant";
        case DamageType.Slashing:
          return "Tranchant";
      }

      return "";
    }
  }
}
