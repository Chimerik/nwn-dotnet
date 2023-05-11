using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static class ItemUtils
  {
    public static readonly int[] shopBasicMagicScrolls = new int[] { NWScript.IP_CONST_CASTSPELL_ACID_SPLASH_1, NWScript.IP_CONST_CASTSPELL_DAZE_1, NWScript.IP_CONST_CASTSPELL_ELECTRIC_JOLT_1, NWScript.IP_CONST_CASTSPELL_FLARE_1, NWScript.IP_CONST_CASTSPELL_RAY_OF_FROST_1, NWScript.IP_CONST_CASTSPELL_RESISTANCE_5, NWScript.IP_CONST_CASTSPELL_BURNING_HANDS_5, NWScript.IP_CONST_CASTSPELL_CHARM_PERSON_2, NWScript.IP_CONST_CASTSPELL_COLOR_SPRAY_2, NWScript.IP_CONST_CASTSPELL_ENDURE_ELEMENTS_2, NWScript.IP_CONST_CASTSPELL_EXPEDITIOUS_RETREAT_5, NWScript.IP_CONST_CASTSPELL_GREASE_2, 459, 478, 460, NWScript.IP_CONST_CASTSPELL_MAGE_ARMOR_2, NWScript.IP_CONST_CASTSPELL_MAGIC_MISSILE_5, NWScript.IP_CONST_CASTSPELL_NEGATIVE_ENERGY_RAY_5, NWScript.IP_CONST_CASTSPELL_RAY_OF_ENFEEBLEMENT_2, NWScript.IP_CONST_CASTSPELL_SCARE_2, 469, NWScript.IP_CONST_CASTSPELL_SHIELD_5, NWScript.IP_CONST_CASTSPELL_SLEEP_5, NWScript.IP_CONST_CASTSPELL_SUMMON_CREATURE_I_5, NWScript.IP_CONST_CASTSPELL_AMPLIFY_5, NWScript.IP_CONST_CASTSPELL_BALAGARNSIRONHORN_7, NWScript.IP_CONST_CASTSPELL_LESSER_DISPEL_5, NWScript.IP_CONST_CASTSPELL_CURE_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_INFLICT_MINOR_WOUNDS_1, NWScript.IP_CONST_CASTSPELL_VIRTUE_1, NWScript.IP_CONST_CASTSPELL_BANE_5, NWScript.IP_CONST_CASTSPELL_BLESS_2, NWScript.IP_CONST_CASTSPELL_CURE_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_DIVINE_FAVOR_5, NWScript.IP_CONST_CASTSPELL_DOOM_5, NWScript.IP_CONST_CASTSPELL_ENTROPIC_SHIELD_5, NWScript.IP_CONST_CASTSPELL_INFLICT_LIGHT_WOUNDS_5, NWScript.IP_CONST_CASTSPELL_REMOVE_FEAR_2, NWScript.IP_CONST_CASTSPELL_SANCTUARY_2, NWScript.IP_CONST_CASTSPELL_SHIELD_OF_FAITH_5, NWScript.IP_CONST_CASTSPELL_CAMOFLAGE_5, NWScript.IP_CONST_CASTSPELL_ENTANGLE_5, NWScript.IP_CONST_CASTSPELL_MAGIC_FANG_5, 540, 541, 542, 543, 544, 583, 587, 591 };
    public static readonly BaseItemType[] leatherBasicWeaponBlueprints = new BaseItemType[] { BaseItemType.Belt, BaseItemType.Gloves, BaseItemType.Boots, BaseItemType.Cloak, BaseItemType.Whip };
    public static readonly BaseItemType[] woodBasicBlueprints = new BaseItemType[] { BaseItemType.SmallShield, BaseItemType.Club, BaseItemType.Dart, BaseItemType.Bullet, BaseItemType.HeavyCrossbow, BaseItemType.LightCrossbow, BaseItemType.Quarterstaff, BaseItemType.Sling, BaseItemType.Arrow, BaseItemType.Bolt };
    public static readonly BaseItemType[] forgeBasicWeaponBlueprints = new BaseItemType[] { BaseItemType.LightMace, BaseItemType.Helmet, BaseItemType.Dagger, BaseItemType.Morningstar, BaseItemType.ShortSpear, BaseItemType.Sickle, BaseItemType.LightHammer, BaseItemType.LightFlail, BaseItemType.Bracer };
    public static readonly int[] forgeBasicArmorBlueprints = new int[] { 4 };
    public static readonly int[] leatherBasicArmorBlueprints = new int[] { 0, 1, 2, 3 };

    public static readonly Dictionary<BaseItemType, Dictionary<ItemAppearanceWeaponModel, List<NuiComboEntry>>> weaponModelDictionary = new();

    public static readonly Dictionary<BaseItemType, int[,]> itemDamageDictionary = new()
    {
      { BaseItemType.Dagger,  new int[,] { { 1, 3 }, { 1, 4 }, { 2, 5 }, { 3, 6 }, { 3, 7 }, { 4, 8 }, { 4, 10 }, { 5, 12 } } },
      { BaseItemType.LightCrossbow,  new int[,] { { 4, 7 }, { 6, 10 }, { 8, 13 }, { 10, 16 }, { 12, 19 }, { 14, 22 }, { 17, 26 }, { 20, 30 } } },
      { BaseItemType.Shortbow,  new int[,] { { 2, 4 }, { 3, 6 }, { 4, 8 }, { 5, 10 }, { 6, 12 }, { 7, 14 }, { 8, 16 }, { 10, 19 } } },
      { BaseItemType.Sling,  new int[,] { { 1, 3 }, { 2, 4 }, { 3, 5 }, { 4, 6 }, { 4, 7 }, { 5, 9 }, { 6, 11 }, { 7, 13 } } },
      { BaseItemType.Club,  new int[,] { { 1, 3 }, { 2, 5 }, { 3, 7 }, { 4, 9 }, { 5, 11 }, { 6, 13 }, { 8, 15 }, { 10, 17 } } },
      { BaseItemType.LightMace,  new int[,] { { 1, 3 }, { 2, 5 }, { 3, 7 }, { 4, 9 }, { 5, 11 }, { 6, 13 }, { 8, 15 }, { 10, 17 } } },
      { BaseItemType.Morningstar,  new int[,] { { 1, 3 }, { 2, 5 }, { 3, 7 }, { 4, 9 }, { 5, 11 }, { 6, 13 }, { 8, 15 }, { 10, 17 } } },
      { BaseItemType.LightFlail,  new int[,] { { 1, 3 }, { 2, 5 }, { 3, 7 }, { 4, 9 }, { 5, 11 }, { 6, 13 }, { 8, 15 }, { 10, 17 } } },
      { BaseItemType.Handaxe,  new int[,] { { 1, 4 }, { 1, 6 }, { 2, 8 }, { 2, 10 }, { 3, 12 }, { 3, 14 }, { 4, 16 }, { 4, 19 } } },
      { BaseItemType.Quarterstaff,  new int[,] { { 3, 5 }, { 4, 7 }, { 5, 10 }, { 7, 13 }, { 9, 16 }, { 11, 19 }, { 13, 22 }, { 15, 25 } } },
      { BaseItemType.ShortSpear,  new int[,] { { 3, 5 }, { 4, 8 }, { 5, 11 }, { 6, 14 }, { 8, 17 }, { 10, 20 }, { 12, 23 }, { 14, 27 } } },
      { BaseItemType.LightHammer,  new int[,] { { 3, 5 }, { 4, 7 }, { 5, 10 }, { 7, 13 }, { 9, 16 }, { 11, 19 }, { 13, 22 }, { 15, 25 } } },
      { BaseItemType.Shortsword,  new int[,] { { 1, 4 }, { 2, 5 }, { 3, 6 }, { 4, 7 }, { 5, 9 }, { 6, 11 }, { 8, 13 }, { 10, 15 } } },
      { BaseItemType.Gloves,  new int[,] { { 1, 3 }, { 1, 4 }, { 2, 5 }, { 3, 6 }, { 3, 7 }, { 4, 8 }, { 4, 10 }, { 5, 12 } } },
      { BaseItemType.Bracer,  new int[,] { { 1, 3 }, { 1, 4 }, { 2, 5 }, { 3, 6 }, { 3, 7 }, { 4, 8 }, { 4, 10 }, { 5, 12 } } },
      { BaseItemType.ThrowingAxe,  new int[,] { { 3, 5 }, { 3, 8 }, { 3, 11 }, { 4, 14 }, { 4, 17 }, { 5, 20 }, { 5, 24 }, { 6, 28 } } },
      { BaseItemType.Battleaxe,  new int[,] { { 3, 5 }, { 3, 8 }, { 3, 11 }, { 4, 14 }, { 4, 17 }, { 5, 20 }, { 5, 24 }, { 6, 28 } } },
      { BaseItemType.Longsword,  new int[,] { { 3, 5 }, { 4, 7 }, { 5, 9 }, { 7, 11 }, { 9, 13 }, { 11, 16 }, { 13, 19 }, { 15, 22 } } },
      { BaseItemType.Scimitar,  new int[,] { { 3, 5 }, { 4, 7 }, { 5, 9 }, { 7, 11 }, { 9, 13 }, { 11, 16 }, { 13, 19 }, { 15, 22 } } },
      { BaseItemType.Rapier,  new int[,] { { 1, 4 }, { 2, 5 }, { 3, 6 }, { 4, 8 }, { 5, 9 }, { 6, 11 }, { 8, 13 }, { 10, 15 } } },
      { BaseItemType.HeavyCrossbow,  new int[,] { { 5, 9 }, { 8, 14 }, { 12, 19 }, { 15, 23 }, { 19, 28 }, { 22, 32 }, { 26, 36 }, { 30, 40 } } },
      { BaseItemType.Warhammer,  new int[,] { { 3, 5 }, { 5, 9 }, { 7, 13 }, { 10, 17 }, { 12, 21 }, { 14, 25 }, { 16, 29 }, { 19, 35 } } },
      { BaseItemType.Greataxe,  new int[,] { { 3, 9 }, { 4, 13 }, { 5, 17 }, { 6, 21 }, { 7, 25 }, { 8, 30 }, { 10, 35 }, { 12, 40 } } },
      { BaseItemType.Greatsword,  new int[,] { { 4, 7 }, { 7, 11 }, { 10, 15 }, { 13, 19 }, { 16, 23 }, { 19, 27 }, { 22, 31 }, { 25, 35 } } },
      { BaseItemType.Halberd,  new int[,] { { 3, 9 }, { 4, 13 }, { 5, 17 }, { 6, 21 }, { 7, 25 }, { 8, 30 }, { 10, 35 }, { 12, 40 } } },
      { BaseItemType.HeavyFlail,  new int[,] { { 3, 5 }, { 5, 9 }, { 7, 13 }, { 10, 17 }, { 12, 21 }, { 14, 25 }, { 16, 29 }, { 19, 35 } } },
      { BaseItemType.Longbow,  new int[,] { { 3, 5 }, { 4, 8 }, { 5, 11 }, { 7, 14 }, { 9, 17 }, { 11, 20 }, { 13, 24 }, { 15, 28 } } },
      { BaseItemType.Trident,  new int[,] { { 3, 5 }, { 4, 8 }, { 5, 11 }, { 6, 14 }, { 8, 17 }, { 10, 20 }, { 12, 23 }, { 14, 27 } } },
      { BaseItemType.Kama,  new int[,] { { 2, 4 }, { 2, 5 }, { 3, 6 }, { 4, 7 }, { 4, 8 }, { 5, 10 }, { 6, 13 }, { 8, 16 } } },
      { BaseItemType.Kukri,  new int[,] { { 2, 4 }, { 2, 5 }, { 3, 6 }, { 4, 7 }, { 4, 8 }, { 5, 10 }, { 6, 13 }, { 8, 16 } } },
      { BaseItemType.Bastardsword,  new int[,] { { 4, 6 }, { 6, 9 }, { 8, 12 }, { 10, 15 }, { 12, 18 }, { 14, 21 }, { 17, 25 }, { 20, 29 } } },
      { BaseItemType.DwarvenWaraxe,  new int[,] { { 4, 6 }, { 4, 10 }, { 4, 14 }, { 5, 18 }, { 5, 22 }, { 6, 26 }, { 7, 31 }, { 8, 36 } } },
      { BaseItemType.Katana,  new int[,] { { 4, 6 }, { 6, 9 }, { 8, 12 }, { 10, 15 }, { 12, 18 }, { 14, 21 }, { 17, 25 }, { 20, 29 } } },
      { BaseItemType.DireMace,  new int[,] { { 3, 5 }, { 4, 7 }, { 5, 10 }, { 7, 13 }, { 9, 16 }, { 11, 19 }, { 13, 22 }, { 15, 25 } } },
      { BaseItemType.Doubleaxe,  new int[,] { { 3, 5 }, { 3, 8 }, { 3, 11 }, { 4, 14 }, { 4, 17 }, { 5, 20 }, { 5, 24 }, { 6, 28 } } },
      { BaseItemType.TwoBladedSword,  new int[,] { { 3, 5 }, { 4, 7 }, { 3, 9 }, { 7, 11 }, { 9, 13 }, { 11, 16 }, { 13, 19 }, { 15, 22 } } },
    };

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
      return baseItemType switch
      {
        BaseItemType.Armor or BaseItemType.Helmet => ItemCategory.Armor,
        BaseItemType.SmallShield or BaseItemType.TowerShield or BaseItemType.LargeShield => ItemCategory.Shield,
        BaseItemType.Doubleaxe or BaseItemType.Greataxe or BaseItemType.Greatsword or BaseItemType.Halberd or BaseItemType.HeavyFlail or BaseItemType.Quarterstaff or BaseItemType.Scythe or BaseItemType.TwoBladedSword or BaseItemType.DireMace or BaseItemType.Trident or BaseItemType.ShortSpear => ItemCategory.TwoHandedMeleeWeapon,
        BaseItemType.Bastardsword or BaseItemType.Longsword or BaseItemType.Battleaxe or BaseItemType.Club or BaseItemType.Dagger or BaseItemType.DwarvenWaraxe or BaseItemType.Handaxe or BaseItemType.Kama or BaseItemType.Katana or BaseItemType.Kukri or BaseItemType.LightFlail or BaseItemType.LightHammer or BaseItemType.LightMace or BaseItemType.Morningstar or BaseItemType.Rapier or BaseItemType.Shortsword or BaseItemType.Scimitar or BaseItemType.Sickle or BaseItemType.Warhammer or BaseItemType.Whip => ItemCategory.OneHandedMeleeWeapon,
        BaseItemType.HeavyCrossbow or BaseItemType.LightCrossbow or BaseItemType.Shortbow or BaseItemType.Longbow or BaseItemType.Dart or BaseItemType.Sling or BaseItemType.ThrowingAxe => ItemCategory.RangedWeapon,
        BaseItemType.Arrow or BaseItemType.Bolt or BaseItemType.Bullet => ItemCategory.Ammunition,
        BaseItemType.Potions or BaseItemType.BlankPotion or BaseItemType.EnchantedPotion => ItemCategory.Potions,
        BaseItemType.Scroll or BaseItemType.BlankScroll or BaseItemType.EnchantedScroll or BaseItemType.SpellScroll => ItemCategory.Scroll,
        BaseItemType.Belt or BaseItemType.Boots or BaseItemType.Bracer or BaseItemType.Cloak or BaseItemType.Gloves => ItemCategory.Clothes,
        //marteau de forgeron
        (BaseItemType)114 or (BaseItemType)115 => ItemCategory.CraftTool,
        _ => ItemCategory.Invalid,
      };
    }
    public static bool CanBeEquippedInLeftHand(BaseItemType baseItemType)
    {
      return baseItemType switch
      {
        BaseItemType.Dagger or BaseItemType.Kama or BaseItemType.Kukri or BaseItemType.Sickle or BaseItemType.Shortsword or BaseItemType.Handaxe
        or BaseItemType.LightFlail or BaseItemType.Torch or BaseItemType.SmallShield or BaseItemType.LargeShield or BaseItemType.TowerShield => true,
        _ => false
      };
    }
    public static bool IsVersatileWeapon(BaseItemType baseItemType)
    {
      return baseItemType switch
      {
        BaseItemType.Club or BaseItemType.Bastardsword or BaseItemType.DwarvenWaraxe or BaseItemType.Katana => true,
        _ => false
      };
    }
    public static bool IsReachWeapon(BaseItemType baseItemType)
    {
      return baseItemType switch
      {
        BaseItemType.Quarterstaff or BaseItemType.Warhammer or BaseItemType.Greataxe or BaseItemType.Greatsword or BaseItemType.Halberd or BaseItemType.HeavyFlail
        or BaseItemType.Trident or BaseItemType.Whip or BaseItemType.Bastardsword or BaseItemType.Scythe => true,
        _ => false
      };
    }
    public static int GetWeaponAttackPerRound(BaseItemType baseItemType)
    {
      return baseItemType switch
      {
        BaseItemType.Dagger or BaseItemType.Sickle or BaseItemType.Handaxe or BaseItemType.Rapier or BaseItemType.Kama or BaseItemType.Kukri 
        or BaseItemType.DireMace or BaseItemType.Doubleaxe or BaseItemType.TwoBladedSword or BaseItemType.Shuriken or BaseItemType.Dart
        or BaseItemType.Longsword or BaseItemType.Scimitar or BaseItemType.Shortsword or BaseItemType.Battleaxe or BaseItemType.Bastardsword
        or BaseItemType.DwarvenWaraxe or BaseItemType.Katana => 5,
        BaseItemType.Club or BaseItemType.LightMace or BaseItemType.Morningstar or BaseItemType.ShortSpear or BaseItemType.LightFlail
        or BaseItemType.Trident or BaseItemType.Whip or BaseItemType.Scythe => 4,
        BaseItemType.Longbow => 2,
        BaseItemType.LightCrossbow or BaseItemType.HeavyCrossbow => 2,
        _ => 3,
      };
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
      float baseCost;

      if (item.BaseItem.ItemType == BaseItemType.Armor)
        baseCost = NwGameTables.ArmorTable.GetRow(item.BaseACValue).Cost.Value;
      else
        baseCost = item.BaseItem.BaseCost;

      if (baseCost <= 0)
      {
        if (item.BaseItem.ItemType != BaseItemType.CreatureItem)
          LogUtils.LogMessage($"{item.Name} - baseCost introuvable pour baseItemType : {item.BaseItem.ItemType}", LogUtils.LogType.ModuleAdministration);

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
      return ipDamageType switch
      {
        IPDamageType.Bludgeoning => DamageType.Bludgeoning,
        IPDamageType.Piercing => DamageType.Piercing,
        IPDamageType.Slashing => DamageType.Slashing,
        IPDamageType.Acid => DamageType.Acid,
        IPDamageType.Magical => DamageType.Magical,
        IPDamageType.Fire => DamageType.Fire,
        IPDamageType.Cold => DamageType.Cold,
        IPDamageType.Electrical => DamageType.Electrical,
        IPDamageType.Divine => DamageType.Divine,
        IPDamageType.Negative => DamageType.Negative,
        IPDamageType.Positive => DamageType.Positive,
        IPDamageType.Sonic => DamageType.Sonic,
        IPDamageType.Physical => (DamageType)8192,// Physical
        (IPDamageType)14 => (DamageType)16384,// Elemental
        _ => DamageType.Slashing,
      };
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

      if(receiver.Inventory.CheckFit(deserializedItem))
        receiver.AcquireItem(deserializedItem);
      else if(receiver.IsLoginPlayerCharacter && PlayerSystem.Players.TryGetValue(receiver, out PlayerSystem.Player player)
        && NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").Any(b => b.GetObjectVariable<LocalVariableInt>("ownerId").Value == player.characterId))
      {
        TradeSystem.AddItemToPlayerDataBaseBank(player.characterId.ToString(), new List<string> { itemTemplate }, "Inventory full");
        receiver.LoginPlayer.SendServerMessage($"L'objet {deserializedItem.Name.ColorString(ColorConstants.White)} ne rentre pas dans votre inventaire. Afin d'éviter toute perte, il a été mis en sécurité dans votre coffre Skalsgard", ColorConstants.Red);
      }
      return deserializedItem;
    }
    public static void CreateShopSkillBook(NwItem skillBook, int featId)
    {
      skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
      skillBook.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value = featId;

      try
      {
        Learnable learnable = SkillSystem.learnableDictionary[featId];
        skillBook.Name = $"Livre de compétence : {learnable.name}";
        skillBook.Description = learnable.description;
        skillBook.BaseGoldValue = (uint)(learnable.multiplier * 100000);
      }
      catch (Exception)
      {
        LogUtils.LogMessage($"ERROR - Could not find {featId} in the learnable dictionnary.", LogUtils.LogType.ModuleAdministration);
        skillBook.Destroy();
      }
    }
    public static void CreateShopWeaponBlueprint(NwItem oBlueprint, NwBaseItem baseItem)
    {
      oBlueprint.Name = $"Patron original : {baseItem.Name}";
      oBlueprint.Description = $"Ce patron contient toutes les instructions de conception, à partir de matéria, pour un objet de type : {baseItem.Name}";

      oBlueprint.BaseGoldValue = (uint)(baseItem.BaseCost * 5000);
      oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value = (int)baseItem.Id;
      oBlueprint.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value = BaseItems2da.baseItemTable[(int)baseItem.ItemType].workshop;
    }
    public static void CreateShopArmorBlueprint(NwItem oBlueprint, int baseArmor)
    {
      var entry = Armor2da.armorTable[baseArmor];

      oBlueprint.Name = $"Patron original : {entry.name}";
      oBlueprint.Description = $"Ce patron contient toutes les instructions de conception, à partir de matéria, pour un objet de type : {entry.name}";

      oBlueprint.BaseGoldValue = (uint)(entry.cost * 5000);
      oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value = (int)BaseItemType.Armor;
      oBlueprint.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value = baseArmor;
      oBlueprint.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value = entry.workshop;
    }
    public static string DisplayDamageType(NwItem item)
    {
      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Polaire: return "Polaire";
          case CustomInscription.Sismique: return "Terrestre";
          case CustomInscription.Incendiaire: return "Feu";
          case CustomInscription.Electrocution: return "Foudre";
        }

      string damageTypeLabel = "";

      foreach (DamageType damageType in item.BaseItem.WeaponType)
        damageTypeLabel += $"{ItemUtils.DisplayBaseDamageType(damageType)} / ";

      return damageTypeLabel.Remove(damageTypeLabel.Length - 2);
    }
    public static string DisplayBaseDamageType(DamageType damageType)
    {
      return damageType switch
      {
        DamageType.Bludgeoning => "Contondant",
        DamageType.Piercing => "Perforant",
        DamageType.Slashing => "Tranchant",
        _ => "",
      };
    }
    public static string GetResourceNameFromBlueprint(NwItem blueprint)
    {
      BaseItemType baseItemType = (BaseItemType)blueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
      string workshop = baseItemType == BaseItemType.Armor ? Armor2da.GetWorkshop(blueprint.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value) : BaseItems2da.baseItemTable[(int)baseItemType].workshop;

      return workshop switch
      {
        "forge" => ResourceType.Ingot.ToDescription(),
        "scierie" => ResourceType.Plank.ToDescription(),
        "tannerie" => ResourceType.Leather.ToDescription(),
        _ => "ressource non définie",
      };
    }
    public static ResourceType GetResourceTypeFromBlueprint(NwItem blueprint)
    {
      BaseItemType baseItemType = (BaseItemType)blueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
      string workshop = baseItemType == BaseItemType.Armor ? Armor2da.GetWorkshop(blueprint.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value) : BaseItems2da.baseItemTable[(int)baseItemType].workshop;

      return GetResourceFromWorkshopTag(workshop);
    }
    public static ResourceType GetResourceFromWorkshopTag(string workshop)
    {
      return workshop switch
      {
        "forge" => ResourceType.Ingot,
        "scierie" => ResourceType.Plank,
        "tannerie" => ResourceType.Leather,
        _ => ResourceType.Invalid,
      };
    }
    public static ResourceType GetResourceTypeFromItem(NwItem item)
    {
      string workshop = item.BaseItem.ItemType == BaseItemType.Armor ? Armor2da.GetWorkshop(item.BaseACValue) : BaseItems2da.baseItemTable[(int)item.BaseItem.ItemType].workshop;

      return workshop switch
      {
        "forge" => ResourceType.Ingot,
        "scierie" => ResourceType.Plank,
        "tannerie" => ResourceType.Leather,
        _ => ResourceType.Invalid,
      };
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
    public static void OpenItemCustomizationWindow(NwItem item, PlayerSystem.Player player)
    {
      switch (item.BaseItem.ModelType)
      {
        case BaseItemModelType.Simple:
          if (!player.windows.ContainsKey("simpleItemAppearanceModifier")) player.windows.Add("simpleItemAppearanceModifier", new PlayerSystem.Player.SimpleItemAppearanceWindow(player, item));
          else ((PlayerSystem.Player.SimpleItemAppearanceWindow)player.windows["simpleItemAppearanceModifier"]).CreateWindow(item);
          break;

        case BaseItemModelType.Layered:
          if (item.BaseItem.ItemType == BaseItemType.Helmet)
          {
            if (!player.windows.ContainsKey("helmetColorsModifier")) player.windows.Add("helmetColorsModifier", new PlayerSystem.Player.HelmetCustomizationWindow(player, item));
            else ((PlayerSystem.Player.HelmetCustomizationWindow)player.windows["helmetColorsModifier"]).CreateWindow(item);
          }
          else if (item.BaseItem.ItemType == BaseItemType.Cloak)
          {
            if (!player.windows.ContainsKey("cloakColorsModifier")) player.windows.Add("cloakColorsModifier", new PlayerSystem.Player.CloakCustomizationWindow(player, item));
            else ((PlayerSystem.Player.CloakCustomizationWindow)player.windows["cloakColorsModifier"]).CreateWindow(item);
          }
          break;

        case BaseItemModelType.Composite:
          if (!player.windows.ContainsKey("weaponAppearanceModifier")) player.windows.Add("weaponAppearanceModifier", new PlayerSystem.Player.WeaponAppearanceWindow(player, item));
          else ((PlayerSystem.Player.WeaponAppearanceWindow)player.windows["weaponAppearanceModifier"]).CreateWindow(item);
          break;

        case BaseItemModelType.Armor:
          if (!player.windows.ContainsKey("itemColorsModifier")) player.windows.Add("itemColorsModifier", new PlayerSystem.Player.ArmorCustomizationWindow(player, item));
          else ((PlayerSystem.Player.ArmorCustomizationWindow)player.windows["itemColorsModifier"]).CreateWindow(item);
          break;
      }
    }
    public static List<NuiComboEntry> GetWeaponModelList(BaseItemType baseItem, ItemAppearanceWeaponModel part)
    {
      return weaponModelDictionary[baseItem][part];
    }
    public static void HandleCraftToolDurability(PlayerSystem.Player player, NwItem craftTool, int type, int resourceDurabilitySkill)
    {
      List<ObjectVariable> localsToRemove = new();
      int slotToAdd = 0;
      int skillPoints = player.learnableSkills.ContainsKey(resourceDurabilitySkill) ? player.learnableSkills[resourceDurabilitySkill].totalPoints * 2 : 0;

      for (int i = 0; i < craftTool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        if (craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == type)
          skillPoints += 2;
        else if (craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == type + 1)
          skillPoints += 4;
        else if (craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == type + 2)
          skillPoints += 6;
        else if (craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == type + 3)
          skillPoints += 8;
      }

      for (int i = 0; i < craftTool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing || craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}_DURABILITY").HasNothing 
          || NwRandom.Roll(Utils.random, 100) < skillPoints)
          continue;

        if (NwRandom.Roll(Utils.random, 100) < craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}_DURABILITY").Value)
          craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}_DURABILITY").Value -= 10;
        else
        {
          switch (craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
          {
            case CustomInscription.MateriaDetectionDurabilityMinor: 
            case CustomInscription.MateriaDetectionDurability: 
            case CustomInscription.MateriaDetectionDurabilityMajor: 
            case CustomInscription.MateriaDetectionDurabilitySupreme: 
              player.oid.SendServerMessage($"L'inscription d'amélioration de durabilité de détection de votre {craftTool.Name} est épuisée", ColorConstants.Red); 
              break;

            case CustomInscription.MateriaDetectionAccuracyMinor:
            case CustomInscription.MateriaDetectionAccuracy:
            case CustomInscription.MateriaDetectionAccuracyMajor:
            case CustomInscription.MateriaDetectionAccuracySupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de précision de détection de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaDetectionQualityMinor:
            case CustomInscription.MateriaDetectionQuality:
            case CustomInscription.MateriaDetectionQualityMajor:
            case CustomInscription.MateriaDetectionQualitySupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de qualité de détection de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaDetectionReliabilityMinor:
            case CustomInscription.MateriaDetectionReliability:
            case CustomInscription.MateriaDetectionReliabilityMajor:
            case CustomInscription.MateriaDetectionReliabilitySupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de fiabilité de détection de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaDetectionSpeedMinor:
            case CustomInscription.MateriaDetectionSpeed:
            case CustomInscription.MateriaDetectionSpeedMajor:
            case CustomInscription.MateriaDetectionSpeedSupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de vitesse de détection de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaExtractionDurabilityMinor:
            case CustomInscription.MateriaExtractionDurability:
            case CustomInscription.MateriaExtractionDurabilityMajor:
            case CustomInscription.MateriaExtractionDurabilitySupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de durabilité d'extraction de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaExtractionQualityMinor:
            case CustomInscription.MateriaExtractionQuality:
            case CustomInscription.MateriaExtractionQualityMajor:
            case CustomInscription.MateriaExtractionQualitySupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de qualité d'extraction de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaExtractionSpeedMinor:
            case CustomInscription.MateriaExtractionSpeed:
            case CustomInscription.MateriaExtractionSpeedMajor:
            case CustomInscription.MateriaExtractionSpeedSupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de vitesse d'extraction, de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaExtractionYieldMinor:
            case CustomInscription.MateriaExtractionYield:
            case CustomInscription.MateriaExtractionYieldMajor:
            case CustomInscription.MateriaExtractionYieldSupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de rendement d'extraction, de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaProductionDurabilityMinor:
            case CustomInscription.MateriaProductionDurability:
            case CustomInscription.MateriaProductionDurabilityMajor:
            case CustomInscription.MateriaProductionDurabilitySupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de durabilité de production artisanale de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaProductionYieldMinor:
            case CustomInscription.MateriaProductionYield:
            case CustomInscription.MateriaProductionYieldMajor:
            case CustomInscription.MateriaProductionYieldSupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration d'efficacité de production artisanale de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaProductionSpeedMinor:
            case CustomInscription.MateriaProductionSpeed:
            case CustomInscription.MateriaProductionSpeedMajor:
            case CustomInscription.MateriaProductionSpeedSupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de rapidité de production artisanale de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaInscriptionDurabilityMinor:
            case CustomInscription.MateriaInscriptionDurability:
            case CustomInscription.MateriaInscriptionDurabilityMajor:
            case CustomInscription.MateriaInscriptionDurabilitySupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de durabilité d'inscription de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaInscriptionYieldMinor:
            case CustomInscription.MateriaInscriptionYield:
            case CustomInscription.MateriaInscriptionYieldMajor:
            case CustomInscription.MateriaInscriptionYieldSupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration d'efficacité d'inscription de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;

            case CustomInscription.MateriaInscriptionSpeedMinor:
            case CustomInscription.MateriaInscriptionSpeed:
            case CustomInscription.MateriaInscriptionSpeedMajor:
            case CustomInscription.MateriaInscriptionSpeedSupreme:
              player.oid.SendServerMessage($"L'inscription d'amélioration de rapidité d'inscription de votre {craftTool.Name} est épuisée", ColorConstants.Red);
              break;
          }

          slotToAdd++;
          localsToRemove.Add(craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}_DURABILITY"));
          localsToRemove.Add(craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}"));
        }   
      }

      foreach (var local in localsToRemove)
        local.Delete();

      craftTool.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += slotToAdd;
    }
    private static async void DelayedLocalVarDeletion(ObjectVariable local)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));
      local.Delete();
    }

    public static void GetArmorProperties(NwItem item, List<string> ipNames, List<Color> ipColors)
    {
      int armor = item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;
      int physicalArmor = item.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR");
      int elementalArmor = item.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR");
      int damageAbsorption = 0;
      int prismatique = 0;
      int artisan = 0;
      int gardeDragon = 0;
      int gardeExterieur = 0;
      int gardeAberration = 0;
      int persecuteur = 0;
      int marcheVent = 0;
      int gardeGeant = 0;
      int gardeMagie = 0;
      int gardeBon = 0;
      int gardeChaos = 0;
      int gardeMal = 0;
      int gardeNeutre = 0;
      int gardeLoi = 0;
      int hivernal = 0;
      int ignifuge = 0;
      int paratonnerre = 0;
      int tectonique = 0;
      int infiltrateur = 0;
      int saboteur = 0;
      int avantGarde = 0;
      int gardeHalfelin = 0;
      int gardeHumain = 0;
      int gardeDemiElfe = 0;
      int gardeDemiOrc = 0;
      int gardeElfe = 0;
      int gardeGnome = 0;
      int gardeNain = 0;
      int agitateur = 0;
      int sentinelle = 0;
      int belluaire = 0;
      int eclaireur = 0;
      int disciple = 0;
      int virtuose = 0;
      int fossoyeur = 0;
      int prodige = 0;
      int destructeur = 0;
      int benediction = 0;
      int centurion = 0;
      int oublie = 0;
      int gardeNonVie = 0;
      int gardeArtifice = 0;
      int gardeOrc = 0;
      int lieutenant = 0;
      int maitreBleme = 0;
      int marionnettiste = 0;
      int chaman = 0;
      int gardeMonstre = 0;
      int gardeHumanoide = 0;
      int gardeMetamorphe = 0;
      int gardeGobelinoide = 0;
      int gardeAnimal = 0;
      int gardeReptilien = 0;
      int gardeVermine = 0;


      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Cuirassé: armor += 1; break;
          case CustomInscription.Absorption:
            if (damageAbsorption < 3)
              damageAbsorption += 1;
            break;
          case CustomInscription.Prismatique: prismatique += 1; break;
          case CustomInscription.Artisan: artisan += 1; break;
          case CustomInscription.GardeDragon: gardeDragon += 3; break;
          case CustomInscription.GardeExtérieur: gardeExterieur += 3; break;
          case CustomInscription.GardeAberration: gardeAberration += 3; break;
          case CustomInscription.Persécuteur:
            armor += 1;
            persecuteur += 1;
            break;
          case CustomInscription.Inflexible: physicalArmor += 1; break;
          case CustomInscription.Redoutable: elementalArmor += 1; break;
          case CustomInscription.Marchevent: marcheVent += 1; break;
          case CustomInscription.GardeMagie: gardeMagie += 3; break;
          case CustomInscription.GardeBon: gardeBon += 2; break;
          case CustomInscription.GardeChaos: gardeChaos += 2; break;
          case CustomInscription.GardeMal: gardeMal += 2; break;
          case CustomInscription.GardeNeutre: gardeNeutre += 2; break;
          case CustomInscription.GardeLoi: gardeLoi += 2; break;
          case CustomInscription.GardeGeant: gardeGeant += 3; break;
          case CustomInscription.Hivernal: hivernal += 2; break;
          case CustomInscription.Ignifugé: ignifuge += 2; break;
          case CustomInscription.Paratonnerre: paratonnerre += 2; break;
          case CustomInscription.Tectonique: tectonique += 2; break;
          case CustomInscription.Infiltrateur: infiltrateur += 2; break;
          case CustomInscription.Saboteur: saboteur += 1; break;
          case CustomInscription.AvantGarde: avantGarde += 1; break;
          case CustomInscription.GardeHalfelin: gardeHalfelin += 3; break;
          case CustomInscription.GardeHumain: gardeHumain += 3; break;
          case CustomInscription.GardeDemiElfe: gardeDemiElfe += 3; break;
          case CustomInscription.GardeDemiOrc: gardeDemiOrc += 3; break;
          case CustomInscription.GardeElfe: gardeElfe += 3; break;
          case CustomInscription.GardeGnome: gardeGnome += 3; break;
          case CustomInscription.GardeNain: gardeNain += 3; break;
          case CustomInscription.Agitateur: agitateur += 1; break;
          case CustomInscription.Sentinelle: sentinelle += 1; break;
          case CustomInscription.Belluaire: belluaire += 1; break;
          case CustomInscription.Eclaireur: eclaireur += 1; break;
          case CustomInscription.Disciple: disciple += 2; break;
          case CustomInscription.Virtuose: virtuose += 2; break;
          case CustomInscription.Fossoyeur: fossoyeur += 1; break;
          case CustomInscription.Prodige: prodige += 1; break;
          case CustomInscription.Destructeur: destructeur += 3; break;
          case CustomInscription.Bénédiction: benediction += 1; break;
          case CustomInscription.Centurion: centurion += 1; break;
          case CustomInscription.Oublié: oublie += 1; break;
          case CustomInscription.GardeNonVie: gardeNonVie += 3; break;
          case CustomInscription.GardeArtifice: gardeArtifice += 3; break;
          case CustomInscription.GardeOrc: gardeOrc += 3; break;
          case CustomInscription.Lieutenant:
            lieutenant += 2;
            armor -= 4;
            break;
          case CustomInscription.MaîtreBlème: maitreBleme += 3; break;
          case CustomInscription.Marionnettiste: marionnettiste += 3; break;
          case CustomInscription.Chaman: chaman += 3; break;
          case CustomInscription.GardeMonstre: gardeMonstre += 3; break;
          case CustomInscription.GardeHumanoïde: gardeHumanoide += 3; break;
          case CustomInscription.GardeGoblinoïde: gardeGobelinoide += 3; break;
          case CustomInscription.GardeAnimal: gardeOrc += 3; break;
          case CustomInscription.GardeReptilien: gardeReptilien += 3; break;
          case CustomInscription.GardeVermine: gardeVermine += 3; break;
          case CustomInscription.GardeMétamorphe: gardeMetamorphe += 3; break;
        }
      }

      if (armor != 0)
      {
        ipNames.Add($"+{armor} armure");
        ipColors.Add(ColorConstants.White);
      }

      if (physicalArmor != 0)
      {
        ipNames.Add($"+{physicalArmor} armure physique");
        ipColors.Add(ColorConstants.White);
      }

      if (elementalArmor != 0)
      {
        ipNames.Add($"+{elementalArmor} armure élémentaire");
        ipColors.Add(ColorConstants.White);
      }

      if (damageAbsorption != 0)
      {
        ipNames.Add($"+{damageAbsorption} absorption physique");
        ipColors.Add(ColorConstants.White);
      }

      if (prismatique != 0)
      {
        ipNames.Add($"+{prismatique} armure si maîtrise de l'air > 14");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{prismatique} armure si maîtrise de la terre > 14");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{prismatique} armure si maîtrise du feu > 14");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{prismatique} armure si maîtrise de l'eau > 14");
        ipColors.Add(ColorConstants.White);
      }

      if (artisan != 0)
      {
        ipNames.Add($"+{artisan} armure pour chaque sceau équipé");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeDragon != 0)
      {
        ipNames.Add($"+{gardeDragon} armure contre les Dragons");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeAberration != 0)
      {
        ipNames.Add($"+{gardeAberration} armure contre les Aberrations");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeExterieur != 0)
      {
        ipNames.Add($"+{gardeExterieur} armure contre les Extérieurs");
        ipColors.Add(ColorConstants.White);
      }

      if (persecuteur != 0)
      {
        ipNames.Add($"+6 dégâts divins reçus");
        ipColors.Add(ColorConstants.White);
      }

      if (marcheVent != 0)
      {
        ipNames.Add($"+{marcheVent} armure sous l'effet de 3 sorts positifs");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{marcheVent * 2} armure sous l'effet de 4 sorts positifs");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{marcheVent * 3} armuresous l'effet de 5 sorts positifs");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{marcheVent * 4} armure sous l'effet de 6 sorts positifs");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeGeant != 0)
      {
        ipNames.Add($"+{gardeGeant} armure contre les Géants");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeMagie != 0)
      {
        ipNames.Add($"+{gardeMagie} armure contre les créatures magiques");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeBon != 0)
      {
        ipNames.Add($"+{gardeBon} armure contre les créatures alignées au Bien");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeChaos != 0)
      {
        ipNames.Add($"+{gardeChaos} armure contre les créatures alignées au Chaos");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeMal != 0)
      {
        ipNames.Add($"+{gardeMal} armure contre les créatures alignées au Mal");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeNeutre != 0)
      {
        ipNames.Add($"+{gardeNeutre} armure contre les créatures alignées de façon Neutre");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeLoi != 0)
      {
        ipNames.Add($"+{gardeLoi} armure contre les créatures alignées à la Loi");
        ipColors.Add(ColorConstants.White);
      }

      if (hivernal != 0)
      {
        ipNames.Add($"+{hivernal} armure contre les dégâts Polaires");
        ipColors.Add(ColorConstants.White);
      }

      if (ignifuge != 0)
      {
        ipNames.Add($"+{ignifuge} armure contre les dégâts de Feu");
        ipColors.Add(ColorConstants.White);
      }

      if (paratonnerre != 0)
      {
        ipNames.Add($"+{paratonnerre} armure contre les dégâts de Foudre");
        ipColors.Add(ColorConstants.White);
      }

      if (tectonique != 0)
      {
        ipNames.Add($"+{tectonique} armure contre les dégâts Terrestres");
        ipColors.Add(ColorConstants.White);
      }

      if (infiltrateur != 0)
      {
        ipNames.Add($"+{infiltrateur} armure contre les dégâts Perforants");
        ipColors.Add(ColorConstants.White);
      }

      if (saboteur != 0)
      {
        ipNames.Add($"+{saboteur} armure contre les dégâts Tranchants");
        ipColors.Add(ColorConstants.White);
      }

      if (avantGarde != 0)
      {
        ipNames.Add($"+{avantGarde} armure contre les dégâts Contondants");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeHalfelin != 0)
      {
        ipNames.Add($"+{gardeHalfelin} armure contre les Halfelins");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeHumain != 0)
      {
        ipNames.Add($"+{gardeHumain} armure contre les Humains");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeDemiElfe != 0)
      {
        ipNames.Add($"+{gardeDemiElfe} armure contre les Demi-Elfes");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeDemiOrc != 0)
      {
        ipNames.Add($"+{gardeDemiOrc} armure contre les Demi-Orcs");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeElfe != 0)
      {
        ipNames.Add($"+{gardeElfe} armure contre les Elfes");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeGnome != 0)
      {
        ipNames.Add($"+{gardeGnome} armure contre les Gnomes");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeNain != 0)
      {
        ipNames.Add($"+{gardeNain} armure contre les Nains");
        ipColors.Add(ColorConstants.White);
      }

      if (agitateur != 0)
      {
        ipNames.Add($"+{agitateur} armure en attaquant");
        ipColors.Add(ColorConstants.White);
      }

      if (sentinelle != 0)
      {
        ipNames.Add($"+{sentinelle} armure sous pose de combat");
        ipColors.Add(ColorConstants.White);
      }

      if (belluaire != 0)
      {
        ipNames.Add($"+{belluaire} armure tant que votre compagnon animal est en vie");
        ipColors.Add(ColorConstants.White);
      }

      if (eclaireur != 0)
      {
        ipNames.Add($"+{eclaireur} armure sous l'effet d'une préparation");
        ipColors.Add(ColorConstants.White);
      }

      if (disciple != 0)
      {
        ipNames.Add($"+{disciple} armure sous l'effet d'une condition");
        ipColors.Add(ColorConstants.White);
      }

      if (virtuose != 0)
      {
        ipNames.Add($"+{virtuose} armure en incantant un sort");
        ipColors.Add(ColorConstants.White);
      }

      if (fossoyeur != 0)
      {
        ipNames.Add($"+{fossoyeur} armure si santé < 80%");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{fossoyeur * 2} armure si santé < 60%");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{fossoyeur * 3} armure si santé < 40%");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{fossoyeur * 4} armure si santé < 20%");
        ipColors.Add(ColorConstants.White);
      }

      if (prodige != 0)
      {
        ipNames.Add($"+{prodige} armure tant que vous rechargez une capacité");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{prodige * 2} armure tant que vous rechargez une capacité");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{prodige * 3} armure tant que vous rechargez une capacité");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{prodige * 4} armure tant que vous rechargez une capacité");
        ipColors.Add(ColorConstants.White);
      }

      if (destructeur != 0)
      {
        ipNames.Add($"+{destructeur} armure sous l'effet d'un maléfice");
        ipColors.Add(ColorConstants.White);
      }

      if (benediction != 0)
      {
        ipNames.Add($"+{benediction} armure sous l'effet d'un sort positif");
        ipColors.Add(ColorConstants.White);
      }

      if (centurion != 0)
      {
        ipNames.Add($"+{centurion} armure sous l'effet d'un cri, écho ou chant");
        ipColors.Add(ColorConstants.White);
      }

      if (oublie != 0)
      {
        ipNames.Add($"+{oublie} armure tant que vous n'êtes pas sous l'effet d'un sort positif");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeNonVie != 0)
      {
        ipNames.Add($"+{gardeNonVie} armure contre les Mort-Vivants");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeArtifice != 0)
      {
        ipNames.Add($"+{gardeArtifice} armure contre les créatures Artificielles");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeOrc != 0)
      {
        ipNames.Add($"+{gardeOrc} armure contre les Orcs");
        ipColors.Add(ColorConstants.White);
      }

      if (lieutenant != 0)
      {
        ipNames.Add($"-{lieutenant}% de durée des maléfices vous affectant");
        ipColors.Add(ColorConstants.White);
      }

      if (maitreBleme != 0)
      {
        ipNames.Add($"+{maitreBleme} armure tant que vous contrôlez 1 mort-vivant");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{maitreBleme * 2} armure tant que vous contrôlez 3 mort-vivants");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{maitreBleme * 3} armure tant que vous contrôlez 5 mort-vivants");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{maitreBleme * 4} armure tant que vous contrôlez 8 mort-vivants");
        ipColors.Add(ColorConstants.White);
      }

      if (marionnettiste != 0)
      {
        ipNames.Add($"+{marionnettiste} armure tant que vous contrôlez 1 invocation");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{marionnettiste * 2} armure tant que vous contrôlez 3 invocations");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{marionnettiste * 3} armure tant que vous contrôlez 5 invocations");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{marionnettiste * 4} armure tant que vous contrôlez 8 invocations");
        ipColors.Add(ColorConstants.White);
      }

      if (chaman != 0)
      {
        ipNames.Add($"+{chaman} armure tant que vous contrôlez 1 esprit");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{chaman * 2} armure tant que vous contrôlez 2 esprits");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{chaman * 3} armure tant que vous contrôlez 3 esprits");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"+{chaman * 4} armure tant que vous contrôlez 4 esprits");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeMonstre != 0)
      {
        ipNames.Add($"+{gardeMonstre} armure contre les Monstres Primitifs");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeHumanoide != 0)
      {
        ipNames.Add($"+{gardeHumanoide} armure contre les créatures Humanoïdes");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeMetamorphe != 0)
      {
        ipNames.Add($"+{gardeMetamorphe} armure contre les Métamorphes");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeGobelinoide != 0)
      {
        ipNames.Add($"+{gardeGobelinoide} armure contre les Gobelinoïdes");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeAnimal != 0)
      {
        ipNames.Add($"+{gardeAnimal} armure contre les Animaux");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeReptilien != 0)
      {
        ipNames.Add($"+{gardeReptilien} armure contre les créatures Reptiliennes");
        ipColors.Add(ColorConstants.White);
      }

      if (gardeVermine != 0)
      {
        ipNames.Add($"+{gardeVermine} armure contre la Vermine");
        ipColors.Add(ColorConstants.White);
      }
    }
    public static void GetShieldProperties(NwItem item, List<string> ipNames, List<Color> ipColors)
    {
      int armor = item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;
      int piercingArmor = item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR");
      int repousseDragon = 0;
      int repousseExterieur = 0;
      int repousseAberration = 0;
      int longueVieAuRoi = 0;
      int laFoiEstMonBouclier = 0;
      int laSurvieDuMieuxEquipe = 0;
      int pareEnTouteSaison = 0;
      int repousseGeant = 0;
      int repousseMagie = 0;
      int repousseBon = 0;
      int repousseChaos = 0;
      int repousseNeutre = 0;
      int repousseMal = 0;
      int repousseLoi = 0;
      int contreVentsEtMarees = 0;
      int lEnigmeDelAcier = 0;
      int pasLeVisage = 0;
      int porteParLeVent = 0;
      int commeUnRoc = 0;
      int illumination = 0;
      int chevaucheLaTempete = 0;
      int repousseHalfelin = 0;
      int repousseHumain = 0;
      int repousseDemiElfe = 0;
      int repousseDemiOrc = 0;
      int repousseElfe = 0;
      int repousseGnome = 0;
      int repousseNain = 0;
      int laRaisonDuPlusFort = 0;
      int savoirNestQueLaMoitie = 0;
      int ceNestQuuneEgratignure = 0;
      int neTremblezPas = 0;
      int vigueur = 0;
      int repousseNonVie = 0;
      int repousseArtifice = 0;
      int repousseOrc = 0;
      int piete = 0;
      int tenacite = 0;
      int determination = 0;
      int simplesdEsprit = 0;
      int laVieNestQueDouleur = 0;
      int repousseMonstre = 0;
      int repousseHumanoide = 0;
      int repousseMetamorphe = 0;
      int repousseGobelinoide = 0;
      int repousseAnimal = 0;
      int repousseReptilien = 0;
      int repousseVermine = 0;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Blindé: armor += 1; break;
          case CustomInscription.RepousseDragon: repousseDragon += 2; break;
          case CustomInscription.RepousseExtérieur: repousseExterieur += 2; break;
          case CustomInscription.RepousseAberration: repousseAberration += 2; break;
          case CustomInscription.LongueVieAuRoi: longueVieAuRoi += 1; break;
          case CustomInscription.LaFoiEstMonBouclier: laFoiEstMonBouclier += 1; break;
          case CustomInscription.LaSurvieDuMieuxEquipé: laSurvieDuMieuxEquipe += 1; break;
          case CustomInscription.ParéEnTouteSaison: pareEnTouteSaison += 1; break;
          case CustomInscription.RepousseGéant: repousseGeant += 1; break;
          case CustomInscription.RepousseMagie: repousseMagie += 1; break;
          case CustomInscription.RepousseBon: repousseBon += 1; break;
          case CustomInscription.RepousseChaos: repousseChaos += 1; break;
          case CustomInscription.RepousseNeutre: repousseNeutre += 1; break;
          case CustomInscription.RepousseMal: repousseMal += 1; break;
          case CustomInscription.RepousseLoi: repousseLoi += 1; break;
          case CustomInscription.ContreVentsEtMarées: contreVentsEtMarees += 2; break;
          case CustomInscription.lEnigmeDelAcier: lEnigmeDelAcier += 2; break;
          case CustomInscription.PasLeVisage: pasLeVisage += 2; break;
          case CustomInscription.PortéParLeVent: porteParLeVent += 2; break;
          case CustomInscription.CommeUnRoc: commeUnRoc += 2; break;
          case CustomInscription.Illumination: illumination += 2; break;
          case CustomInscription.ChevaucheLaTempête: chevaucheLaTempete += 2; break;
          case CustomInscription.RepousseHalfelin: repousseHalfelin += 2; break;
          case CustomInscription.RepousseHumain: repousseHumain += 2; break;
          case CustomInscription.RepousseDemiElfe: repousseDemiElfe += 2; break;
          case CustomInscription.RepousseDemiOrc: repousseDemiOrc += 2; break;
          case CustomInscription.RepousseElfe: repousseElfe += 2; break;
          case CustomInscription.RepousseGnome: repousseGnome += 2; break;
          case CustomInscription.RepousseNain: repousseNain += 2; break;
          case CustomInscription.LaRaisonDuPlusFort: laRaisonDuPlusFort += 1; break;
          case CustomInscription.SavoirNestQueLaMoitiéDuChemin: savoirNestQueLaMoitie += 1; break;
          case CustomInscription.CeNestQuuneEgratignure: ceNestQuuneEgratignure += 2; break;
          case CustomInscription.NeTremblezPas: neTremblezPas += 2; break;
          case CustomInscription.Vigueur: vigueur += 4; break;
          case CustomInscription.RepousseNonVie: repousseNonVie += 2; break;
          case CustomInscription.RepousseArtifice: repousseArtifice += 2; break;
          case CustomInscription.RepousseOrc: repousseOrc += 2; break;
          case CustomInscription.Piété: piete += 6; break;
          case CustomInscription.Ténacité: tenacite += 6; break;
          case CustomInscription.Détermination: determination += 8; break;
          case CustomInscription.HeureuxLesSimplesdEsprits:
            armor += 1;
            simplesdEsprit += 1; 
            break;
          case CustomInscription.LaVieNestQueDouleur:
            armor += 1;
            laVieNestQueDouleur += 3;
            break;
          case CustomInscription.RepousseMonstre: repousseMonstre += 2; break;
          case CustomInscription.RepousseHumanoïde: repousseHumanoide += 2; break;
          case CustomInscription.RepousseMétamorphe: repousseMetamorphe += 2; break;
          case CustomInscription.RepousseGobelinoïde: repousseGobelinoide += 2; break;
          case CustomInscription.RepousseAnimal: repousseAnimal += 2; break;
          case CustomInscription.RepousseReptilien: repousseReptilien += 2; break;
          case CustomInscription.RepousseVermine: repousseVermine += 2; break;
        }
      }

      if (armor != 0)
      {
        ipNames.Add($"+{armor} armure");
        ipColors.Add(ColorConstants.White);
      }

      if (piercingArmor != 0)
      {
        ipNames.Add($"+{armor} armure contre les dégâts Perforants");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseDragon != 0)
      {
        ipNames.Add($"+{repousseDragon} armure contre Dragons");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseAberration != 0)
      {
        ipNames.Add($"+{repousseAberration} armure contre les Aberrations");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseExterieur != 0)
      {
        ipNames.Add($"+{repousseExterieur} armure contre les Extérieurs");
        ipColors.Add(ColorConstants.White);
      }

      if (longueVieAuRoi != 0)
      {
        ipNames.Add($"+{longueVieAuRoi} armure si santé > 50%");
        ipColors.Add(ColorConstants.White);
      }

      if (laFoiEstMonBouclier != 0)
      {
        ipNames.Add($"+{laFoiEstMonBouclier} armure sous l'effet d'un sort positif");
        ipColors.Add(ColorConstants.White);
      }

      if (laSurvieDuMieuxEquipe != 0)
      {
        ipNames.Add($"+{laSurvieDuMieuxEquipe} armure contre les dégâts Physiques");
        ipColors.Add(ColorConstants.White);
      }

      if (pareEnTouteSaison != 0)
      {
        ipNames.Add($"+{pareEnTouteSaison} armure contre les dégâts Elémentaires");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseGeant != 0)
      {
        ipNames.Add($"+{repousseGeant} armure contre les Géants");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseMagie != 0)
      {
        ipNames.Add($"+{repousseMagie} armure contre les créatures Magiques");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseBon != 0)
      {
        ipNames.Add($"+{repousseBon} armure contre les créatures alignées au Bien");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseChaos != 0)
      {
        ipNames.Add($"+{repousseChaos} armure contre les créatures alignées au Chaos");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseMal != 0)
      {
        ipNames.Add($"+{repousseMal} armure contre les créatures alignées au Mal");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseNeutre != 0)
      {
        ipNames.Add($"+{repousseNeutre} armure contre les créatures alignée de façon Neutre");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseLoi != 0)
      {
        ipNames.Add($"+{repousseLoi} armure contre les créatures alignées à la Loi");
        ipColors.Add(ColorConstants.White);
      }

      if (contreVentsEtMarees != 0)
      {
        ipNames.Add($"+{contreVentsEtMarees} armure contre les dégâts Perforants");
        ipColors.Add(ColorConstants.White);
      }

      if (lEnigmeDelAcier != 0)
      {
        ipNames.Add($"+{lEnigmeDelAcier} armure contre les dégâts Tranchants");
        ipColors.Add(ColorConstants.White);
      }

      if (pasLeVisage != 0)
      {
        ipNames.Add($"+{pasLeVisage} armure contre les dégâts Contondants");
        ipColors.Add(ColorConstants.White);
      }

      if (porteParLeVent != 0)
      {
        ipNames.Add($"+{porteParLeVent} armure contre les dégâts Polaires");
        ipColors.Add(ColorConstants.White);
      }

      if (commeUnRoc != 0)
      {
        ipNames.Add($"+{commeUnRoc} armure contre les dégâts Terrestres");
        ipColors.Add(ColorConstants.White);
      }

      if (illumination != 0)
      {
        ipNames.Add($"+{illumination} armure contre les dégâts de Feu");
        ipColors.Add(ColorConstants.White);
      }

      if (chevaucheLaTempete != 0)
      {
        ipNames.Add($"+{chevaucheLaTempete} armure contre les dégâts de Foudre");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseHalfelin != 0)
      {
        ipNames.Add($"+{repousseHalfelin} armure contre les Halfelins");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseHumain != 0)
      {
        ipNames.Add($"+{repousseHumain} armure contre les Humains");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseDemiElfe != 0)
      {
        ipNames.Add($"+{repousseDemiElfe} armure contre les Demi-Elfes");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseDemiOrc != 0)
      {
        ipNames.Add($"+{repousseDemiOrc} armure contre les Demi-Orcs");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseElfe != 0)
      {
        ipNames.Add($"+{repousseElfe} armure contre les Elfes");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseGnome != 0)
      {
        ipNames.Add($"+{repousseGnome} armure contre les Gnomes");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseNain != 0)
      {
        ipNames.Add($"+{repousseNain} armure contre les Nains");
        ipColors.Add(ColorConstants.White);
      }

      if (laRaisonDuPlusFort != 0)
      {
        ipNames.Add($"+{laRaisonDuPlusFort} armure en attaquant");
        ipColors.Add(ColorConstants.White);
      }

      if (savoirNestQueLaMoitie != 0)
      {
        ipNames.Add($"+{savoirNestQueLaMoitie} armure en incantant un sort");
        ipColors.Add(ColorConstants.White);
      }

      if (ceNestQuuneEgratignure != 0)
      {
        ipNames.Add($"+{ceNestQuuneEgratignure} armure si Santé < 50%");
        ipColors.Add(ColorConstants.White);
      }

      if (neTremblezPas != 0)
      {
        ipNames.Add($"+{neTremblezPas} armure sous l'effet d'un maléfice");
        ipColors.Add(ColorConstants.White);
      }

      if (vigueur != 0)
      {
        ipNames.Add($"+{vigueur} Santé");
        ipColors.Add(ColorConstants.White);
      }

      if (piete != 0)
      {
        ipNames.Add($"+{piete} Santé sous l'effet d'un sort positif");
        ipColors.Add(ColorConstants.White);
      }

      if (tenacite != 0)
      {
        ipNames.Add($"+{tenacite} Santé sous l'effet d'une pose de combat");
        ipColors.Add(ColorConstants.White);
      }

      if (determination != 0)
      {
        ipNames.Add($"+{determination} Santé sous l'effet d'un maléfice");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseNonVie != 0)
      {
        ipNames.Add($"+{repousseNonVie} armure contre les Mort-Vivants");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseArtifice != 0)
      {
        ipNames.Add($"+{repousseArtifice} armure contre les créatures Artificielles");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseOrc != 0)
      {
        ipNames.Add($"+{repousseOrc} armure contre les Orcs");
        ipColors.Add(ColorConstants.White);
      }

      if (simplesdEsprit != 0)
      {
        ipNames.Add($"-{simplesdEsprit} énergie");
        ipColors.Add(ColorConstants.White);
      }

      if (laVieNestQueDouleur != 0)
      {
        ipNames.Add($"-{laVieNestQueDouleur} Santé");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseMonstre != 0)
      {
        ipNames.Add($"+{repousseMonstre} armure contre les créatures Monstrueuses");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseHumanoide != 0)
      {
        ipNames.Add($"+{repousseHumanoide} armure contre les créatures Humanoïdes");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseMetamorphe != 0)
      {
        ipNames.Add($"+{repousseMetamorphe} armure contre les Métamorphes");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseGobelinoide != 0)
      {
        ipNames.Add($"+{repousseGobelinoide} armure contre les Gobelinoïdes");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseAnimal != 0)
      {
        ipNames.Add($"+{repousseAnimal} armure contre les Animaux");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseReptilien != 0)
      {
        ipNames.Add($"+{repousseReptilien} armure contre les créatures Reptiliennes");
        ipColors.Add(ColorConstants.White);
      }

      if (repousseVermine != 0)
      {
        ipNames.Add($"+{repousseVermine} armure contre la Vermine");
        ipColors.Add(ColorConstants.White);
      }
    }
    public static void GetJewelProperties(NwItem item, List<string> ipNames, List<Color> ipColors)
    {
      int erreurs = 0;
      int patience = 0;
      int evocateur = 0;
      int ecoute = 0;
      int poingDeFer = 0;
      int ensanglante = 0;
      int invocateur = 0;
      int belliciste = 0;
      int raison = 0;
      int opportuniste = 0;
      int resilience = 0;
      int clarte = 0;
      int purete = 0;
      int recuperation = 0;
      int cicatrisant = 0;
      int ardeur = 0;
      int mithridate = 0;
      int coagulant = 0;
      int hardiesse = 0;
      int survivant = 0;
      int rayonnant = 0;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.OnApprendDeSesErreurs: erreurs += 1; break;
          case CustomInscription.PatienceEtLongueurDeTemps: patience += 1; break;
          case CustomInscription.Evocateur: evocateur += 1; break;
          case CustomInscription.AuxDependsDeCeluiQuilEcoute: ecoute += 1; break;
          case CustomInscription.PoingDeFer: poingDeFer += 1; break;
          case CustomInscription.Ensanglanté: ensanglante += 2; break;
          case CustomInscription.Invocateur: invocateur += 2; break;
          case CustomInscription.Belliciste: belliciste += 1; break;
          case CustomInscription.LaMeilleureDesRaisons: raison += 1; break;
          case CustomInscription.Opportuniste: opportuniste += 1; break;
          case CustomInscription.Résilence: resilience += 1; break;
          case CustomInscription.Clarté: clarte += 5; break;
          case CustomInscription.Pureté: purete += 5; break;
          case CustomInscription.Récupération: recuperation += 5; break;
          case CustomInscription.Cicatrisant: cicatrisant += 5; break;
          case CustomInscription.Ardeur: ardeur += 5; break;
          case CustomInscription.Mithridate: mithridate += 5; break;
          case CustomInscription.Coagulant: coagulant += 5; break;
          case CustomInscription.Hardiesse: hardiesse += 5; break;
          case CustomInscription.Survivant: survivant += 1; break;
          case CustomInscription.Rayonnant: rayonnant += 1; break;
        }
      }

      if (erreurs != 0)
      {
        ipNames.Add($"+{erreurs} intelligence");
        ipColors.Add(ColorConstants.White);
      }

      if (patience != 0)
      {
        ipNames.Add($"+{patience} sagesse");
        ipColors.Add(ColorConstants.White);
      }

      if (ecoute != 0)
      {
        ipNames.Add($"+{ecoute} charisme");
        ipColors.Add(ColorConstants.White);
      }

      if (raison != 0)
      {
        ipNames.Add($"+{raison} fort");
        ipColors.Add(ColorConstants.White);
      }

      if (opportuniste != 0)
      {
        ipNames.Add($"+{opportuniste} dextérité");
        ipColors.Add(ColorConstants.White);
      }

      if (resilience != 0)
      {
        ipNames.Add($"+{resilience} constitution");
        ipColors.Add(ColorConstants.White);
      }

      if (evocateur != 0)
      {
        ipNames.Add($"+{evocateur} emplacement de sort");
        ipColors.Add(ColorConstants.White);
      }

      if (poingDeFer != 0)
      {
        ipNames.Add($"+{poingDeFer}s durée de renversement (max 3s)");
        ipColors.Add(ColorConstants.White);
      }

      if (ensanglante != 0)
      {
        ipNames.Add($"-{ensanglante}% de temps d'incantation des sorts exploitant des corps");
        ipColors.Add(ColorConstants.White);
      }

      if (invocateur != 0)
      {
        ipNames.Add($"-{invocateur}% de temps d'incantation des sorts d'invocation");
        ipColors.Add(ColorConstants.White);
      }

      if (belliciste != 0)
      {
        ipNames.Add($"+{belliciste} emplacement de combat");
        ipColors.Add(ColorConstants.White);
      }

      if (clarte != 0)
      {
        ipNames.Add($"-{clarte}% durée des effets d'aveuglement");
        ipColors.Add(ColorConstants.White);
      }

      if (purete != 0)
      {
        ipNames.Add($"-{purete}% durée des effets de maladie");
        ipColors.Add(ColorConstants.White);
      }

      if (recuperation != 0)
      {
        ipNames.Add($"-{recuperation}% durée des effets d'étourdissement");
        ipColors.Add(ColorConstants.White);
      }

      if (cicatrisant != 0)
      {
        ipNames.Add($"-{cicatrisant}% durée des effets de blessure profonde");
        ipColors.Add(ColorConstants.White);
      }

      if (ardeur != 0)
      {
        ipNames.Add($"-{ardeur}% durée des effets de faiblesse");
        ipColors.Add(ColorConstants.White);
      }

      if (mithridate != 0)
      {
        ipNames.Add($"-{mithridate}% durée des effets d'empoisonnement");
        ipColors.Add(ColorConstants.White);
      }

      if (coagulant != 0)
      {
        ipNames.Add($"-{coagulant}% durée des effets de saignement");
        ipColors.Add(ColorConstants.White);
      }

      if (hardiesse != 0)
      {
        ipNames.Add($"-{hardiesse}% durée des effets d'infirmité");
        ipColors.Add(ColorConstants.White);
      }

      if (survivant != 0)
      {
        ipNames.Add($"+{survivant} Santé");
        ipColors.Add(ColorConstants.White);
      }

      if (rayonnant != 0)
      {
        ipNames.Add($"+{rayonnant} Energie");
        ipColors.Add(ColorConstants.White);
      }
    }
    public static void GetWeaponProperties(NwItem item, List<string> ipNames, List<Color> ipColors)
    {
      int pourfendeur = 0;
      int defense = 0;
      int courage = 0;
      int fulgurance = 0;
      int extension = 0;
      int vampirisme = 0;
      int zele = 0;
      int pourfendeurDragon = 0;
      int pourfendeurExterieur = 0;
      int pourfendeurAberration = 0;
      int forceEtHonneur = 0;
      int maitreDeSonDestin = 0;
      int danseAvecLaMort = 0;
      int sadisme = 0;
      int masochisme = 0;
      int queDuMuscle = 0;
      int refuge = 0;
      int protecteur = 0;
      int devotion = 0;
      int endurance = 0;
      int valeur = 0;
      int serenite = 0;
      int toutAuTalent = 0;
      int vision = 0;
      int pourfendeurGeant = 0;
      int pourfendeurMagie = 0;
      int pourfendeurBien = 0;
      int pourfendeurChaos = 0;
      int pourfendeurMal = 0;
      int pourfendeurNeutralite = 0;
      int pourfendeurLoi = 0;
      int vengeanceSeraMienne = 0;
      int faucheuse = 0;
      int givroclaste = 0;
      int pyroclaste = 0;
      int electroclaste = 0;
      int seismoclaste = 0;
      int aiguillon = 0;
      int rasoir = 0;
      int fracasseur = 0;
      int pourfendeurHalfelin = 0;
      int pourfendeurDemiElfe = 0;
      int pourfendeurDemiOrc = 0;
      int pourfendeurHumain = 0;
      int pourfendeurElfe = 0;
      int pourfendeurGnome = 0;
      int pourfendeurNain = 0;
      int securite = 0;
      int ayezFoi = 0;
      int aucunRecours = 0;
      int mienneEstLaPeine = 0;
      int carpeDiem = 0;
      int fureur = 0;
      int penetration = 0;
      int pourfendeurOrc = 0;
      int pourfendeurNonVie = 0;
      int pourfendeurArtificiel = 0;
      int maitrise = 0;
      int adepte = 0;
      int barbele = 0;
      int atrocite = 0;
      int handicapant = 0;
      int pesanteur = 0;
      int venimeuse = 0;
      int mutisme = 0;
      int pourfendeurMonstre = 0;
      int pourfendeurHumanoide = 0;
      int pourfendeurMetamorphe = 0;
      int pourfendeurGobelin = 0;
      int pourfendeurAnimal = 0;
      int pourfendeurReptilien = 0;
      int pourfendeurVermine = 0;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Pourfendeur: pourfendeur += 1; break;
          case CustomInscription.Défense: defense += 1; break;
          case CustomInscription.Courage: courage += 4; break;
          case CustomInscription.Fulgurance: fulgurance += 1; break;
          case CustomInscription.Extension: extension += 6; break;
          case CustomInscription.Vampirisme: vampirisme += 1; break;
          case CustomInscription.Zèle: zele += 1; break;
          case CustomInscription.PourfendeurDragon: zele += 3; break;
          case CustomInscription.PourfendeurExtérieur: zele += 3; break;
          case CustomInscription.PourfendeurAberration: zele += 3; break;
          case CustomInscription.ForceEtHonneur: forceEtHonneur += 2; break;
          case CustomInscription.MaîtreDeSonDestin: maitreDeSonDestin += 2; break;
          case CustomInscription.DanseAvecLaMort: danseAvecLaMort += 2; break;
          case CustomInscription.Sadisme: sadisme += 3; break;
          case CustomInscription.Masochisme: 
            masochisme += 1;
            pourfendeur += 2;
            break;
          case CustomInscription.QueDuMuscle:
            queDuMuscle += 1;
            pourfendeur += 2;
            break;
          case CustomInscription.Refuge: refuge += 1; break;
          case CustomInscription.Protecteur: protecteur += 1; break;
          case CustomInscription.Dévotion: devotion += 6; break;
          case CustomInscription.Endurance: endurance += 6; break;
          case CustomInscription.Valeur: valeur += 8; break;
          case CustomInscription.Sérénité: serenite += 1; break;
          case CustomInscription.ToutAuTalent: toutAuTalent += 1; break;
          case CustomInscription.Vision: vision += 1; break;
          case CustomInscription.PourfendeurGéant: pourfendeurGeant += 3; break;
          case CustomInscription.PourfendeurMagie: pourfendeurMagie += 3; break;
          case CustomInscription.PourfendeurBien: pourfendeurBien += 2; break;
          case CustomInscription.PourfendeurChaos: pourfendeurChaos += 2; break;
          case CustomInscription.PourfendeurMal: pourfendeurMal += 2; break;
          case CustomInscription.PourfendeurNeutralité: pourfendeurNeutralite += 2; break;
          case CustomInscription.PourfendeurLoi: pourfendeurLoi += 2; break;
          case CustomInscription.VengeanceSeraMienne: vengeanceSeraMienne += 3; break;
          case CustomInscription.AccueillezLaFaucheuse: faucheuse += 3; break;
          case CustomInscription.Givroclaste: givroclaste += 2; break;
          case CustomInscription.Pyroclaste: pyroclaste += 2; break;
          case CustomInscription.Electroclaste: electroclaste += 2; break;
          case CustomInscription.Séismoclaste: seismoclaste += 2; break;
          case CustomInscription.Aiguillon: aiguillon += 2; break;
          case CustomInscription.Rasoir: rasoir += 2; break;
          case CustomInscription.Fracasseur: fracasseur += 2; break;
          case CustomInscription.PourfendeurHalfelin: pourfendeurHalfelin += 3; break;
          case CustomInscription.PourfendeurHumain: pourfendeurHumain += 3; break;
          case CustomInscription.PourfendeurDemiElfe: pourfendeurDemiElfe += 3; break;
          case CustomInscription.PourfendeurDemiOrc: pourfendeurDemiOrc += 3; break;
          case CustomInscription.PourfendeurElfe: pourfendeurElfe += 3; break;
          case CustomInscription.PourfendeurGnome: pourfendeurGnome += 3; break;
          case CustomInscription.PourfendeurNain: pourfendeurNain += 3; break;
          case CustomInscription.LaSécuritéAvantTout: securite += 1; break;
          case CustomInscription.AyezFoi: securite += 1; break;
          case CustomInscription.AucunRecours: aucunRecours += 2; break;
          case CustomInscription.MienneEstLaPeine: mienneEstLaPeine += 2; break;
          case CustomInscription.Fureur: fureur += 1; break;
          case CustomInscription.Pénétration: penetration += 3; break;
          case CustomInscription.PourfendeurNonVie: pourfendeurNonVie += 3; break;
          case CustomInscription.PourfendeurArtificiel: pourfendeurArtificiel += 3; break;
          case CustomInscription.PourfendeurOrc: pourfendeurOrc += 3; break;
          case CustomInscription.Maîtrise: pourfendeurOrc += 3; break;
          case CustomInscription.Adepte: adepte += 3; break;
          case CustomInscription.Barbelé: barbele += 4; break;
          case CustomInscription.Atrocité: atrocite += 4; break;
          case CustomInscription.Handicapant: handicapant += 4; break;
          case CustomInscription.Pesanteur: pesanteur += 4; break;
          case CustomInscription.Venimeuse: venimeuse += 4; break;
          case CustomInscription.Mutisme: mutisme += 4; break;
          case CustomInscription.PourfendeurMonstres: mutisme += 4; break;
          case CustomInscription.PourfendeurHumanoïdes: mutisme += 4; break;
          case CustomInscription.PourfendeurMétamorphes: mutisme += 4; break;
          case CustomInscription.PourfendeurGobelins: mutisme += 4; break;
          case CustomInscription.PourfendeurAnimal: mutisme += 4; break;
          case CustomInscription.PourfendeurReptilien: mutisme += 4; break;
          case CustomInscription.PourfendeurVermine: mutisme += 4; break;
        }
      }

      if (pourfendeur != 0)
      {
        ipNames.Add($"+{pourfendeur}% Dégâts");
        ipColors.Add(ColorConstants.White);
      }

      if (defense != 0)
      {
        ipNames.Add($"+{defense} Armure");
        ipColors.Add(ColorConstants.White);
      }

      if (courage != 0)
      {
        ipNames.Add($"+{courage} Santé");
        ipColors.Add(ColorConstants.White);
      }

      if (fulgurance != 0)
      {
        ipNames.Add($"{fulgurance}% de chance de diviser par deux le temps d'incantation des sorts");
        ipColors.Add(ColorConstants.White);
      }

      if (extension != 0)
      {
        ipNames.Add($"+{extension}% de durée des sorts positifs que vous lancez");
        ipColors.Add(ColorConstants.White);
      }

      if (vampirisme != 0)
      {
        ipNames.Add($"+3 drain de vie (non cumulable)");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"-1 récupération d'énergie");
        ipColors.Add(ColorConstants.White);
      }

      if (zele != 0)
      {
        ipNames.Add($"+1 Energie au toucher");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"-1 récupération d'énergie");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurDragon != 0)
      {
        ipNames.Add($"+{pourfendeurDragon}% Dégâts contre les Dragons");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurExterieur != 0)
      {
        ipNames.Add($"+{pourfendeurExterieur}% Dégâts contre les Extérieurs");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurAberration != 0)
      {
        ipNames.Add($"+{pourfendeurAberration}% Dégâts contre les Aberrations");
        ipColors.Add(ColorConstants.White);
      }

      if (forceEtHonneur != 0)
      {
        ipNames.Add($"+{forceEtHonneur}% Dégâts si la Santé est > 50%");
        ipColors.Add(ColorConstants.White);
      }

      if (maitreDeSonDestin != 0)
      {
        ipNames.Add($"+{maitreDeSonDestin}% Dégâts sous l'effet d'un sort positif");
        ipColors.Add(ColorConstants.White);
      }

      if (danseAvecLaMort != 0)
      {
        ipNames.Add($"+{danseAvecLaMort}% Dégâts sous l'effet d'une pose de combat");
        ipColors.Add(ColorConstants.White);
      }

      if (sadisme != 0)
      {
        ipNames.Add($"+{sadisme}% Dégâts contres le ennemis victimes d'un maléfice");
        ipColors.Add(ColorConstants.White);
      }

      if (masochisme != 0)
      {
        ipNames.Add($"-{masochisme} Armure en attaquant");
        ipColors.Add(ColorConstants.White);
      }

      if (queDuMuscle != 0)
      {
        ipNames.Add($"-{queDuMuscle} Energie");
        ipColors.Add(ColorConstants.White);
      }

      if (refuge != 0)
      {
        ipNames.Add($"+{refuge} Armure contre les dégâts Physiques");
        ipColors.Add(ColorConstants.White);
      }

      if (protecteur != 0)
      {
        ipNames.Add($"+{protecteur} Armure contre les dégâts Elémentaires");
        ipColors.Add(ColorConstants.White);
      }

      if (devotion != 0)
      {
        ipNames.Add($"+{devotion} Santé sous l'effet d'un enchantement");
        ipColors.Add(ColorConstants.White);
      }

      if (endurance != 0)
      {
        ipNames.Add($"+{endurance} Santé sous l'effet d'une pose de combat");
        ipColors.Add(ColorConstants.White);
      }

      if (valeur != 0)
      {
        ipNames.Add($"+{valeur} Santé sous l'effet d'un maléfice");
        ipColors.Add(ColorConstants.White);
      }

      if (serenite != 0)
      {
        ipNames.Add($"+{serenite}% de chance de diviser par deux le temps de recharge des capacités");
        ipColors.Add(ColorConstants.White);
      }

      if (toutAuTalent != 0)
      {
        ipNames.Add($"+{toutAuTalent}% de chance de diviser par deux le temps de recharge des capacités");
        ipColors.Add(ColorConstants.White);
      }

      if (vision != 0)
      {
        ipNames.Add($"+{vision} Energie");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurGeant != 0)
      {
        ipNames.Add($"+{pourfendeurGeant}% Dégâts contre les Géants");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurMagie != 0)
      {
        ipNames.Add($"+{pourfendeurMagie}% Dégâts contre les créatures Magiques");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurBien != 0)
      {
        ipNames.Add($"+{pourfendeurBien}% Dégâts contre les créatures alignées au Bien");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurChaos != 0)
      {
        ipNames.Add($"+{pourfendeurChaos}% Dégâts contre les créatures alignées au Chaos");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurMal != 0)
      {
        ipNames.Add($"+{pourfendeurMal}% Dégâts contre les créatures Magiques alignées au Mal");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurNeutralite != 0)
      {
        ipNames.Add($"+{pourfendeurNeutralite}% Dégâts contre les créatures Magiques alignées de façon Neutre");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurLoi != 0)
      {
        ipNames.Add($"+{pourfendeurLoi}% Dégâts contre les créatures alignées à la Loi");
        ipColors.Add(ColorConstants.White);
      }

      if (vengeanceSeraMienne != 0)
      {
        ipNames.Add($"+{vengeanceSeraMienne}% Dégâts si Santé < 50%");
        ipColors.Add(ColorConstants.White);
      }

      if (faucheuse != 0)
      {
        ipNames.Add($"+{faucheuse}% Dégâts sous l'effet d'un maléfice");
        ipColors.Add(ColorConstants.White);
      }

      if (givroclaste != 0)
      {
        ipNames.Add($"+{givroclaste}% Dégâts Polaires");
        ipColors.Add(ColorConstants.White);
      }

      if (pyroclaste != 0)
      {
        ipNames.Add($"+{pyroclaste}% Dégâts de Feu");
        ipColors.Add(ColorConstants.White);
      }

      if (electroclaste != 0)
      {
        ipNames.Add($"+{electroclaste}% Dégâts de Foudre");
        ipColors.Add(ColorConstants.White);
      }

      if (seismoclaste != 0)
      {
        ipNames.Add($"+{seismoclaste}% Dégâts Terrestres");
        ipColors.Add(ColorConstants.White);
      }

      if (aiguillon != 0)
      {
        ipNames.Add($"+{aiguillon}% Dégâts Perforants");
        ipColors.Add(ColorConstants.White);
      }

      if (rasoir != 0)
      {
        ipNames.Add($"+{rasoir}% Dégâts Tranchants");
        ipColors.Add(ColorConstants.White);
      }

      if (fracasseur != 0)
      {
        ipNames.Add($"+{fracasseur}% Dégâts Contondants");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurHalfelin!= 0)
      {
        ipNames.Add($"+{pourfendeurHalfelin}% Dégâts contre les Hafelins");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurHumain != 0)
      {
        ipNames.Add($"+{pourfendeurHumain}% Dégâts contre les Humains");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurDemiElfe != 0)
      {
        ipNames.Add($"+{pourfendeurDemiElfe}% Dégâts contre les Demi-Elfes");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurDemiOrc != 0)
      {
        ipNames.Add($"+{pourfendeurDemiOrc}% Dégâts contre les Demi-Orcs");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurElfe != 0)
      {
        ipNames.Add($"+{pourfendeurElfe}% Dégâts contre les Elfes");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurGnome != 0)
      {
        ipNames.Add($"+{pourfendeurGnome}% Dégâts contre les Gnomes");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurNain != 0)
      {
        ipNames.Add($"+{pourfendeurNain}% Dégâts contre les Nains");
        ipColors.Add(ColorConstants.White);
      }

      if (securite != 0)
      {
        ipNames.Add($"+{securite} Energie si Santé > 50%");
        ipColors.Add(ColorConstants.White);
      }

      if (ayezFoi != 0)
      {
        ipNames.Add($"+{ayezFoi} Energie sous l'effet d'un sort positif");
        ipColors.Add(ColorConstants.White);
      }

      if (aucunRecours != 0)
      {
        ipNames.Add($"+{aucunRecours} Energie si Santé < 50%");
        ipColors.Add(ColorConstants.White);
      }

      if (mienneEstLaPeine != 0)
      {
        ipNames.Add($"+{mienneEstLaPeine} Energie sous l'effet d'un maléfice");
        ipColors.Add(ColorConstants.White);
      }

      if (carpeDiem != 0)
      {
        ipNames.Add($"+15 Energie (non cumulable)");
        ipColors.Add(ColorConstants.White);
        ipNames.Add($"-1 Récupération d'Energie");
        ipColors.Add(ColorConstants.White);
      }

      if (fureur != 0)
      {
        ipNames.Add($"+{fureur}% chances de doubler le gain d'adrénaline d'une attaque");
        ipColors.Add(ColorConstants.White);
      }

      if (penetration != 0)
      {
        ipNames.Add($"+{penetration}% chances d'ajouter 20% de pénétration d'armure à une attaque");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurNonVie != 0)
      {
        ipNames.Add($"+{pourfendeurNonVie}% Dégâts contre les mort-vivants");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurArtificiel != 0)
      {
        ipNames.Add($"+{pourfendeurArtificiel}% Dégats contre les créatures Artificielles");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurOrc != 0)
      {
        ipNames.Add($"+{pourfendeurOrc}% Dégâts contre les Orcs");
        ipColors.Add(ColorConstants.White);
      }

      if (maitrise != 0)
      {
        ipNames.Add($"+{maitrise}% de chances d'avoir +1 à la compétence de maîtrise de l'arme");
        ipColors.Add(ColorConstants.White);
      }

      if (adepte != 0)
      {
        ipNames.Add($"+{adepte}% de chances de diviser par deux le temps d'incantation des sorts liés à la maîtrise de l'arme");
        ipColors.Add(ColorConstants.White);
      }

      if (barbele != 0)
      {
        ipNames.Add($"+{barbele}% de durée des effets de saignements que vous infligez");
        ipColors.Add(ColorConstants.White);
      }

      if (atrocite != 0)
      {
        ipNames.Add($"+{atrocite}% de durée des effets de blessures profondes que vous infligez");
        ipColors.Add(ColorConstants.White);
      }

      if (handicapant != 0)
      {
        ipNames.Add($"+{handicapant}% de durée des effets d'infirmité que vous infligez");
        ipColors.Add(ColorConstants.White);
      }

      if (pesanteur != 0)
      {
        ipNames.Add($"+{pesanteur}% de durée des effets de faiblesse que vous infligez");
        ipColors.Add(ColorConstants.White);
      }

      if (venimeuse != 0)
      {
        ipNames.Add($"+{venimeuse}% de durée des effets d'empoisonnement que vous infligez");
        ipColors.Add(ColorConstants.White);
      }

      if (mutisme != 0)
      {
        ipNames.Add($"+{mutisme}% de durée des effets d'étourdissement que vous infligez");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurMonstre != 0)
      {
        ipNames.Add($"+{pourfendeurMonstre}% Dégâts contre les créatures Monstrueuses");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurHumanoide != 0)
      {
        ipNames.Add($"+{pourfendeurHumanoide}% Dégâts contre les créatures Humanoïdes");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurMetamorphe != 0)
      {
        ipNames.Add($"+{pourfendeurMetamorphe}% Dégâts contre les Métamorphes");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurGobelin != 0)
      {
        ipNames.Add($"+{pourfendeurGobelin}% Dégâts contre les créatures Gobelinoïdes");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurAnimal != 0)
      {
        ipNames.Add($"+{pourfendeurAnimal}% Dégâts contre les Animaux");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurReptilien != 0)
      {
        ipNames.Add($"+{pourfendeurReptilien}% Dégâts contre les créatures Reptiliennes");
        ipColors.Add(ColorConstants.White);
      }

      if (pourfendeurVermine != 0)
      {
        ipNames.Add($"+{pourfendeurVermine}% Dégâts contre la Vermine");
        ipColors.Add(ColorConstants.White);
      }
    }
  }
}
