using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class CollectSystem
  {
    public static Dictionary<OreType, Ore> oresDictionnary = new Dictionary<OreType, Ore>();

    public partial class Ore
    {
      public OreType type;
      public string name;
      public Feat feat;
      public Dictionary<MineralType, float> mineralsDictionnary = new Dictionary<MineralType, float>();
      public Ore(OreType oreType, Feat oreFeat)
      {
        this.type = oreType;
        this.name = GetNameFromOreType(oreType);
        this.feat = oreFeat;
        this.InitiateOreRefinementYield();
      }

      public void InitiateOreRefinementYield()
      {
        switch (this.type)
        {
          case OreType.Veldspar:
            this.mineralsDictionnary.Add(MineralType.Tritanium, 41.500f);
            break;
          case OreType.Scordite:
            this.mineralsDictionnary.Add(MineralType.Tritanium, 23.067f);
            this.mineralsDictionnary.Add(MineralType.Pyerite, 11.533f);
            break;
          case OreType.Pyroxeres:
            this.mineralsDictionnary.Add(MineralType.Tritanium, 11.700f);
            this.mineralsDictionnary.Add(MineralType.Pyerite, 0.833f);
            this.mineralsDictionnary.Add(MineralType.Mexallon,  1.667f);
            this.mineralsDictionnary.Add(MineralType.Noxcium, 0.167f);
            break;
        }
      }
    }

    public static void InitiateOres()
    {
      oresDictionnary.Add(OreType.Veldspar, new Ore(OreType.Veldspar, Feat.VeldsparReprocessing));
      oresDictionnary.Add(OreType.Scordite, new Ore(OreType.Scordite, Feat.ScorditeReprocessing));
      oresDictionnary.Add(OreType.Pyroxeres, new Ore(OreType.Pyroxeres, Feat.PyroxeresReprocessing));
    }
    public static OreType GetOreTypeFromName(string name)
    {
      switch (name)
      {
        case "Veldspar": return OreType.Veldspar;
        case "Scordite": return OreType.Scordite;
        case "Pyroxeres": return OreType.Pyroxeres;
      }

      return OreType.Invalid;
    }
    public static string GetNameFromOreType(OreType type)
    {
      switch (type)
      {
        case OreType.Veldspar: return "Veldspar";
        case OreType.Scordite: return "Scordite";
        case OreType.Pyroxeres: return "Pyroxeres";
      }

      return "";
    }
    public static MineralType GetMineralTypeFromName(string name)
    {
      switch (name)
      {
        case "Tritanium": return MineralType.Tritanium;
        case "Pyerite": return MineralType.Pyerite;
        case "Mexallon": return MineralType.Mexallon;
        case "Noxcium": return MineralType.Noxcium;
      }

      return MineralType.Invalid;
    }
    public static string GetNameFromMineralType(MineralType type)
    {
      switch (type)
      {
        case MineralType.Tritanium: return "Tritanium";
        case MineralType.Pyerite: return "Pyerite";
        case MineralType.Mexallon: return "Mexallon";
        case MineralType.Noxcium: return "Noxcium";
      }

      return "";
    }
    public enum OreType
    {
      Invalid = 0,
      Veldspar = 1,
      Scordite = 2,
      Pyroxeres = 3,
    }
    public enum MineralType
    {
      Invalid = 0,
      Tritanium = 1,
      Pyerite = 2,
      Mexallon = 3,
      Noxcium = 4,
    }
    // TODO : améliorer / copier / diminution de durabilité du blueprint copié
    public static Dictionary<BlueprintType, Blueprint> blueprintDictionnary = new Dictionary<BlueprintType, Blueprint>();
    public partial class Blueprint
    {
      public BlueprintType type;
      public string workshopTag { get; set; }
      public string craftedItemTag { get; set; }
      public int mineralsCost { get; set; }
      public Feat feat { get; set; }
      public Blueprint(BlueprintType type)
      {
        this.type = type;
        this.InitiateBluePrintCosts();
      }

      public void InitiateBluePrintCosts()
      {
        switch (this.type)
        {
          case BlueprintType.Longsword:
            this.mineralsCost = 15000;
            this.workshopTag = "forge";
            this.craftedItemTag = "longsword";
            this.feat = Feat.ForgeLongsword;
            break;
          case BlueprintType.Fullplate:
            this.mineralsCost = 1500000;
            this.workshopTag = "forge";
            this.craftedItemTag = "fullplate";
            this.feat = Feat.ForgeFullplate;
            break;
        }
      }
    }
    public enum BlueprintType
    {
      Invalid = 0,
      Dagger = 1,
      Longsword = 2,
      Chainshirt = 3,
      Fullplate = 4,
    }
    public static BlueprintType GetBlueprintTypeFromName(string name)
    {
      switch (name)
      {
        case "Longsword": return BlueprintType.Longsword;
        case "Fullplate": return BlueprintType.Fullplate;
        case "Dagger": return BlueprintType.Dagger;
        case "Chainshirt": return BlueprintType.Chainshirt;
      }

      return BlueprintType.Invalid;
    }
    public static string GetNameFromBlueprintType(BlueprintType type)
    {
      switch (type)
      {
        case BlueprintType.Longsword: return "Longsword";
        case BlueprintType.Fullplate: return "Fullplate";
        case BlueprintType.Dagger: return "Dagger";
        case BlueprintType.Chainshirt: return "Chainshirt";
      }

      return "";
    }

    public static ItemProperty[] GetCraftItemProperties(MineralType material, ItemSystem.ItemCategory itemCategory)
    {
      switch (material)
      {
        case MineralType.Tritanium: return GetTritaniumItemProperties();
        case MineralType.Pyerite: return GetPyeriteItemProperties(itemCategory);
      }

      Utils.LogMessageToDMs($"No craft property found for material {material.ToString()} and item {itemCategory.ToString()}");

      return null;
    }
    public static ItemProperty[] GetTritaniumItemProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_50_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_50_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_50_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_15_LBS)
      };
    }
    public static ItemProperty[] GetPyeriteItemProperties(ItemSystem.ItemCategory itemCategory)
    {
      switch (itemCategory)
      {
        case ItemSystem.ItemCategory.OneHandedMeleeWeapon: return GetPyeriteOneHandedMeleeWeaponProperties();
        case ItemSystem.ItemCategory.TwoHandedMeleeWeapon: return GetPyeriteTwoHandedMeleeWeaponProperties();
        case ItemSystem.ItemCategory.Armor: return GetPyeriteArmorProperties();
        case ItemSystem.ItemCategory.Shield: return GetPyeriteShieldProperties();
      }

      return null;
    }
      public static ItemProperty[] GetPyeriteOneHandedMeleeWeaponProperties()
      {
        return new ItemProperty[]
        {
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, 1),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, 1),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, NWScript.DAMAGE_TYPE_BASE_WEAPON, NWScript.DAMAGE_BONUS_1),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, NWScript.DAMAGE_TYPE_BASE_WEAPON, NWScript.DAMAGE_BONUS_1)
        };
      }
    public static ItemProperty[] GetPyeriteTwoHandedMeleeWeaponProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, 2),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, 2),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, NWScript.DAMAGE_TYPE_BASE_WEAPON, NWScript.DAMAGE_BONUS_2),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, NWScript.DAMAGE_TYPE_BASE_WEAPON, NWScript.DAMAGE_BONUS_2)
      };
    }
    public static ItemProperty[] GetPyeriteArmorProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyACBonusVsDmgType(NWScript.DAMAGE_TYPE_BLUDGEONING, 1)
      };
    }
    public static ItemProperty[] GetPyeriteShieldProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.DAMAGE_TYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyACBonusVsDmgType(NWScript.DAMAGE_TYPE_PIERCING, 1)
      };
    }
  }
}
