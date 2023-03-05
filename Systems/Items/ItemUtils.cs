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
        or BaseItemType.DireMace or BaseItemType.Doubleaxe or BaseItemType.TwoBladedSword or BaseItemType.Shuriken => 3,
        BaseItemType.Dart or BaseItemType.Shortbow or BaseItemType.Sling or BaseItemType.Club or BaseItemType.LightMace or BaseItemType.Morningstar
        or BaseItemType.ShortSpear or BaseItemType.LightFlail or BaseItemType.Battleaxe or BaseItemType.Longsword or BaseItemType.Scimitar
        or BaseItemType.Longbow or BaseItemType.Trident or BaseItemType.Whip or BaseItemType.Bastardsword or BaseItemType.DwarvenWaraxe 
        or BaseItemType.Katana => 2,
        _ => 1
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
        Utils.LogMessageToDMs($"ERROR - Could not find {featId} in the learnable dictionnary.");
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
    public static string DisplayDamageType(DamageType damageType)
    {
      return damageType switch
      {
        DamageType.Bludgeoning => "Contondant",
        DamageType.Piercing => "Perçant",
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
    public static void HandleCraftToolDurability(PlayerSystem.Player player, NwItem craftTool, string type, int resourceDurabilitySkill)
    {
      List<ObjectVariable> localsToRemove = new();
      int slotToAdd = 0;
      int skillPoints = player.learnableSkills.ContainsKey(resourceDurabilitySkill) ? player.learnableSkills[resourceDurabilitySkill].totalPoints * 2 : 0;
      skillPoints += craftTool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_{type}_RESIST_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value);

      foreach (var local in craftTool.LocalVariables)
      {
        if (!local.Name.StartsWith($"ENCHANTEMENT_CUSTOM_{type}_") || !local.Name.Contains("_DURABILITY") || NwRandom.Roll(Utils.random, 100) < skillPoints)
          continue;

        LocalVariableInt durabilityVar = (LocalVariableInt)local;
        if (NwRandom.Roll(Utils.random, 100) < durabilityVar.Value)
          durabilityVar.Value -= 10;
        else
        {
          string[] enchantementArray = local.Name.Split("_");

          switch (enchantementArray[3]) // type de l'enchantement
          {
            case "YIELD": 
              
              switch(type)
              {
                case "EXTRACTOR": player.oid.SendServerMessage($"L'enchantement d'amélioration de rendement de votre outil est épuisé", ColorConstants.Red); break;
                case "DETECTOR": player.oid.SendServerMessage($"L'enchantement d'amélioration de sensibilité de votre outil est épuisé", ColorConstants.Red); break;
                case "CRAFT": player.oid.SendServerMessage($"L'enchantement de réduction de coût de matéria de votre outil est épuisé", ColorConstants.Red); break;
              }

              break;
              
            case "SPEED": player.oid.SendServerMessage($"L'enchantement d'amélioration de vitesse de votre outil est épuisé", ColorConstants.Red); break;
            case "QUALITY": player.oid.SendServerMessage($"L'enchantement d'amélioration de précision de votre outil est épuisé", ColorConstants.Red); break;
            case "ACCURACY": player.oid.SendServerMessage($"L'enchantement d'amélioration de qualité de votre outil est épuisé", ColorConstants.Red); break;
            case "RESIST": player.oid.SendServerMessage($"L'enchantement d'amélioration de durabilité de votre outil est épuisé", ColorConstants.Red); break;
          }

          localsToRemove.Add(craftTool.GetObjectVariable<LocalVariableInt>(local.Name.Replace("_DURABILITY", "")));
          //DelayedLocalVarDeletion(craftTool.GetObjectVariable<LocalVariableInt>(local.Name.Replace("_DURABILITY", "")));
          localsToRemove.Add(local);
          //DelayedLocalVarDeletion(local);

          //craftTool.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          slotToAdd++;
          localsToRemove.Add(craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{int.Parse(enchantementArray[5])}"));
          //DelayedLocalVarDeletion(craftTool.GetObjectVariable<LocalVariableInt>($"SLOT{int.Parse(enchantementArray[5])}"));
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
  }
}
