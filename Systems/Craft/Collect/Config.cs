using System;
using System.Collections.Generic;
using NWN.Core;
using static NWN.Systems.Items.Utils;

namespace NWN.Systems.Craft.Collect
{
  public class Config
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
        this.InitializeOreRefinementYield();
      }

      public void InitializeOreRefinementYield()
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

    public static void InitializeOres()
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
    public static ItemProperty[] GetTritaniumItemProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_50_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_50_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_50_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_15_LBS)
      };
    }
    public static ItemProperty[] GetPyeriteItemProperties(ItemCategory itemCategory)
    {
      switch (itemCategory)
      {
        case ItemCategory.OneHandedMeleeWeapon: return GetPyeriteOneHandedMeleeWeaponProperties();
        case ItemCategory.TwoHandedMeleeWeapon: return GetPyeriteTwoHandedMeleeWeaponProperties();
        case ItemCategory.Armor: return GetPyeriteArmorProperties();
        case ItemCategory.Shield: return GetPyeriteShieldProperties();
        case ItemCategory.CraftTool: return GetPyeriteToolProperties();
      }

      return null;
    }
      public static ItemProperty[] GetPyeriteOneHandedMeleeWeaponProperties()
      {
        return new ItemProperty[]
        {
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, 1),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, 1),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, NWScript.IP_CONST_DAMAGETYPE_PHYSICAL, NWScript.DAMAGE_BONUS_1),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, NWScript.IP_CONST_DAMAGETYPE_PHYSICAL, NWScript.DAMAGE_BONUS_1)
        };
      }
    public static ItemProperty[] GetPyeriteTwoHandedMeleeWeaponProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, 2),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, 2),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, NWScript.IP_CONST_DAMAGETYPE_PHYSICAL, NWScript.DAMAGE_BONUS_2),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, NWScript.IP_CONST_DAMAGETYPE_PHYSICAL, NWScript.DAMAGE_BONUS_2)
      };
    }
    public static ItemProperty[] GetPyeriteArmorProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyACBonusVsDmgType(NWScript.IP_CONST_DAMAGETYPE_BLUDGEONING, 1)
      };
    }
    public static ItemProperty[] GetPyeriteShieldProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
          NWScript.ItemPropertyACBonusVsDmgType(NWScript.IP_CONST_DAMAGETYPE_PIERCING, 1)
      };
    }
    public static ItemProperty[] GetPyeriteToolProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyWeightIncrease(NWScript.IP_CONST_WEIGHTINCREASE_10_LBS),
      };
    }
  }
}
