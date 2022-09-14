using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;

using NLog.Fluent;

using NWN.Core;

namespace NWN.Systems
{
  public static class ItemUtils
  {
    public static int[] shopBasicMagicScrolls = new int[] { NWScript.IP_CONST_CASTSPELL_ACID_SPLASH_1, NWScript.IP_CONST_CASTSPELL_DAZE_1, NWScript.IP_CONST_CASTSPELL_ELECTRIC_JOLT_1, NWScript.IP_CONST_CASTSPELL_FLARE_1, NWScript.IP_CONST_CASTSPELL_RAY_OF_FROST_1, NWScript.IP_CONST_CASTSPELL_RESISTANCE_5, NWScript.IP_CONST_CASTSPELL_BURNING_HANDS_5, NWScript.IP_CONST_CASTSPELL_CHARM_PERSON_2, NWScript.IP_CONST_CASTSPELL_COLOR_SPRAY_2, NWScript.IP_CONST_CASTSPELL_ENDURE_ELEMENTS_2, NWScript.IP_CONST_CASTSPELL_EXPEDITIOUS_RETREAT_5, NWScript.IP_CONST_CASTSPELL_GREASE_2, 459, 478, 460, NWScript.IP_CONST_CASTSPELL_MAGE_ARMOR_2, NWScript.IP_CONST_CASTSPELL_MAGIC_MISSILE_5, NWScript.IP_CONST_CASTSPELL_NEGATIVE_ENERGY_RAY_5, NWScript.IP_CONST_CASTSPELL_RAY_OF_ENFEEBLEMENT_2, NWScript.IP_CONST_CASTSPELL_SCARE_2, 469, NWScript.IP_CONST_CASTSPELL_SHIELD_5, NWScript.IP_CONST_CASTSPELL_SLEEP_5, NWScript.IP_CONST_CASTSPELL_SUMMON_CREATURE_I_5, NWScript.IP_CONST_CASTSPELL_AMPLIFY_5, NWScript.IP_CONST_CASTSPELL_BALAGARNSIRONHORN_7, NWScript.IP_CONST_CASTSPELL_LESSER_DISPEL_5, NWScript.IP_CONST_CASTSPELL_CURE_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_INFLICT_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_VIRTUE_1, NWScript.IP_CONST_CASTSPELL_BANE_5, NWScript.IP_CONST_CASTSPELL_BLESS_2, NWScript.IP_CONST_CASTSPELL_CURE_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_DIVINE_FAVOR_5, NWScript.IP_CONST_CASTSPELL_DOOM_5, NWScript.IP_CONST_CASTSPELL_ENTROPIC_SHIELD_5, NWScript.IP_CONST_CASTSPELL_INFLICT_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_REMOVE_FEAR_2, NWScript.IP_CONST_CASTSPELL_SANCTUARY_2, NWScript.IP_CONST_CASTSPELL_SHIELD_OF_FAITH_5, NWScript.IP_CONST_CASTSPELL_CAMOFLAGE_5, NWScript.IP_CONST_CASTSPELL_ENTANGLE_5, NWScript.IP_CONST_CASTSPELL_MAGIC_FANG_5, 540, 541, 542, 543, 544 };
    public static readonly BaseItemType[] leatherBasicWeaponBlueprints = new BaseItemType[] { BaseItemType.Belt, BaseItemType.Gloves, BaseItemType.Boots, BaseItemType.Cloak, BaseItemType.Whip };
    public static readonly BaseItemType[] woodBasicBlueprints = new BaseItemType[] { BaseItemType.SmallShield, BaseItemType.Club, BaseItemType.Dart, BaseItemType.Bullet, BaseItemType.HeavyCrossbow, BaseItemType.LightCrossbow, BaseItemType.Quarterstaff, BaseItemType.Sling, BaseItemType.Arrow, BaseItemType.Bolt };
    public static readonly BaseItemType[] forgeBasicWeaponBlueprints = new BaseItemType[] { BaseItemType.LightMace, BaseItemType.Helmet, BaseItemType.Dagger, BaseItemType.Morningstar, BaseItemType.ShortSpear, BaseItemType.Sickle, BaseItemType.LightHammer, BaseItemType.LightFlail, BaseItemType.Bracer };
    public static readonly int[] forgeBasicArmorBlueprints = new int[] { 4 };
    public static readonly int[] leatherBasicArmorBlueprints = new int[] { 0, 1, 2, 3 };

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
      foreach (ItemProperty ip in oItem.ItemProperties.Where(i => i.Property.PropertyType == nItemPropertyType && (i.DurationType == nItemPropertyDuration || nItemPropertyDuration == (EffectDuration)(-1)) && (i?.SubType?.RowIndex == nItemPropertySubType || nItemPropertySubType == -1)))
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
        sortedIP = oItem.ItemProperties.Where(i => i.Property.PropertyType == ipType && i?.SubType?.RowIndex == ipSubType).ToList();
      else
        sortedIP = oItem.ItemProperties.Where(i => i.Property.PropertyType == ipType).ToList();

      ItemProperty maxIp = sortedIP.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
      int nPropBonus = 0;

      if (maxIp != null)
        nPropBonus = maxIp.IntParams[3];

      return nPropBonus;
    }
    public static int GetBaseItemCost(NwItem item)
    {
      float baseCost = 9999999;

      if (item.BaseItem.ItemType == BaseItemType.Armor)
        baseCost = NwGameTables.ArmorTable.GetRow(item.BaseACValue).Cost.Value;
      else
        baseCost = item.BaseItem.BaseCost;

      if (baseCost <= 0)
      {
        if (item.BaseItem.ItemType != BaseItemType.CreatureItem)
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
      switch (ipDamageType)
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
      catch (Exception)
      {
        Utils.LogMessageToDMs($"ERROR - Could not find {featId} in the learnable dictionnary.");
        skillBook.Destroy();
      }
    }
    public static void CreateShopWeaponBlueprint(NwItem oBlueprint, NwBaseItem baseItem)
    {
      oBlueprint.Name = $"Patron original : {baseItem.Name}";
      oBlueprint.Description = $"Ce patron contient toutes les instructions de conception, à partir de matéria, pour un objet de type : {baseItem.Name}";

      oBlueprint.BaseGoldValue = (uint)(baseItem.BaseCost * 50);
      oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value = (int)baseItem.Id;
      oBlueprint.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value = BaseItems2da.baseItemTable[(int)baseItem.ItemType].workshop;
      oBlueprint.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
    }
    public static void CreateShopArmorBlueprint(NwItem oBlueprint, int baseArmor)
    {
      var entry = Armor2da.armorTable[baseArmor];

      oBlueprint.Name = $"Patron original : {entry.name}";
      oBlueprint.Description = $"Ce patron contient toutes les instructions de conception, à partir de matéria, pour un objet de type : {entry.name}";

      oBlueprint.BaseGoldValue = (uint)(entry.cost * 50);
      oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value = (int)BaseItemType.Armor;
      oBlueprint.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value = baseArmor;
      oBlueprint.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value = entry.workshop;
      oBlueprint.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
    }
    public static string DisplayDamageType(DamageType damageType)
    {
      switch (damageType)
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
    public static string GetResourceNameFromBlueprint(NwItem blueprint)
    {
      BaseItemType baseItemType = (BaseItemType)blueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
      string workshop = baseItemType == BaseItemType.Armor ? Armor2da.GetWorkshop(blueprint.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value) : BaseItems2da.baseItemTable[(int)baseItemType].workshop;

      switch (workshop)
      {
        case "forge":
          return ResourceType.Ingot.ToDescription();
        case "scierie":
          return ResourceType.Plank.ToDescription();
        case "tannerie":
          return ResourceType.Leather.ToDescription();
      }

      return "ressource non définie";
    }
    public static ResourceType GetResourceTypeFromBlueprint(NwItem blueprint)
    {
      BaseItemType baseItemType = (BaseItemType)blueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
      string workshop = baseItemType == BaseItemType.Armor ? Armor2da.GetWorkshop(blueprint.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value) : BaseItems2da.baseItemTable[(int)baseItemType].workshop;

      return GetResourceFromWorkshopTag(workshop);
    }
    public static ResourceType GetResourceFromWorkshopTag(string workshop)
    {
      switch (workshop)
      {
        case "forge":
          return ResourceType.Ingot;
        case "scierie":
          return ResourceType.Plank;
        case "tannerie":
          return ResourceType.Leather;
      }

      return ResourceType.Invalid;
    }
    public static ResourceType GetResourceTypeFromItem(NwItem item)
    {
      string workshop = item.BaseItem.ItemType == BaseItemType.Armor ? Armor2da.GetWorkshop(item.BaseACValue) : BaseItems2da.baseItemTable[(int)item.BaseItem.ItemType].workshop;

      switch (workshop)
      {
        case "forge":
          return ResourceType.Ingot;
        case "scierie":
          return ResourceType.Plank;
        case "tannerie":
          return ResourceType.Leather;
      }

      return ResourceType.Invalid;
    }
    public static async void ScheduleItemForDestruction(NwItem item, double delay)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(delay));
      item.Destroy();
    }
    public static void MakeCreatureInventoryUndroppable(NwCreature creature)
    {
      foreach (var item in creature.Inventory.Items)
        item.Droppable = false;

      foreach (InventorySlot slot in (InventorySlot[])Enum.GetValues(typeof(InventorySlot)))
      {
        NwItem item = creature.GetItemInSlot(slot);

        if (item != null && item.IsValid)
          item.Droppable = false;
      }
    }
  }
}
