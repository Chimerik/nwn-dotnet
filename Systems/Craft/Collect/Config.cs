using System;
using System.Collections.Generic;
using NWN.Core;
using static NWN.Systems.Items.Utils;

namespace NWN.Systems.Craft.Collect
{
  public class Config
  {
    public static Dictionary<OreType, Ore> oresDictionnary = new Dictionary<OreType, Ore>
    {
      {
        OreType.Veldspar,
        new Ore(
          oreType: OreType.Veldspar,
          oreFeat: Feat.VeldsparReprocessing,
          mineralsDictionnary: new Dictionary<MineralType, float>
          {
            { MineralType.Tritanium, 41.500f }
          }
        )
      },
      {
        OreType.Scordite,
        new Ore(
          oreType: OreType.Scordite,
          oreFeat: Feat.ScorditeReprocessing,
          mineralsDictionnary: new Dictionary<MineralType, float>
          {
            { MineralType.Tritanium, 23.067f },
            { MineralType.Pyerite, 11.533f }
          }
        )
      },
      {
        OreType.Pyroxeres,
        new Ore(
          oreType: OreType.Pyroxeres,
          oreFeat: Feat.PyroxeresReprocessing,
          mineralsDictionnary: new Dictionary<MineralType, float>
          {
            { MineralType.Tritanium, 11.700f },
            { MineralType.Pyerite, 0.833f },
            { MineralType.Mexallon,  1.667f },
            { MineralType.Noxcium, 0.167f }
          }
        )
      }
    };

    public static Dictionary<MineralType, Mineral> mineralDictionnary = new Dictionary<MineralType, Mineral>
    { 
      { MineralType.Tritanium, new Mineral(MineralType.Tritanium) },
      { MineralType.Pyerite, new Mineral(MineralType.Pyerite) },
      { MineralType.Noxcium, new Mineral(MineralType.Noxcium) },
      { MineralType.Mexallon, new Mineral(MineralType.Mexallon) },
    };
    public struct Ore
    {
      public OreType type;
      public string name;
      public Feat feat;
      public Dictionary<MineralType, float> mineralsDictionnary;
      public Ore(OreType oreType, Feat oreFeat, Dictionary<MineralType, float> mineralsDictionnary)
      {
        this.type = oreType;
        this.name = Enum.GetName(typeof(OreType), oreType) ?? "";
        this.feat = oreFeat;
        this.mineralsDictionnary = mineralsDictionnary;
      }
    }

    public struct Mineral
    {
      public MineralType type;
      public string name;

      public Mineral(MineralType type)
      {
        this.type = type;
        this.name = Enum.GetName(typeof(MineralType), type) ?? "";
      }
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
    public static Dictionary<WoodType, Wood> woodDictionnary = new Dictionary<WoodType, Wood>();
    public partial class Wood
    {
      public WoodType type;
      public string name;
      public Feat feat;
      public Dictionary<PlankType, float> plankDictionnary = new Dictionary<PlankType, float>();
      public Wood(WoodType oreType, Feat oreFeat)
      {
        this.type = oreType;
        this.name = Enum.GetName(typeof(WoodType), oreType);
        this.feat = oreFeat;
        this.InitializeWoodRefinementYield();
      }

      public void InitializeWoodRefinementYield()
      {
        switch (this.type)
        {
          case WoodType.Laurelin:
            this.plankDictionnary.Add(PlankType.Laurelin, 41.500f);
            break;
          case WoodType.Telperion:
            this.plankDictionnary.Add(PlankType.Telperion, 34.567f);
            break;
          case WoodType.Mallorn:
            this.plankDictionnary.Add(PlankType.Mallorn, 15.000f);
            break;
        }
      }
    }
    public enum WoodType
    {
      Invalid = 0,
      Laurelin = 1, // Silmarillion : capture la lumière divine dorée
      Telperion = 2, // Silmarillion : capture la lumière divine argentée
      Mallorn = 3, 
      Nimloth = 4,
      Oiolaire = 5,
      Qliphoth = 6,
      Ferochene = 7,
      Valinor = 8,
    }
    public enum PlankType
    {
      Invalid = 0,
      Laurelin = 1, // Silmarillion : capture la lumière divine dorée
      Telperion = 2, // Silmarillion : capture la lumière divine argentée
      Mallorn = 3,
      Nimloth = 4,
      Oiolaire = 5,
      Qliphoth = 6,
      Ferochene = 7,
      Valinor = 8,
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
    public static OreType GetRandomOreSpawnFromAreaLevel(int level)
    {
      int random = Utils.random.Next(1, 101);
      switch (level)
      {
        case 2:
          return OreType.Veldspar;
        case 3:
          if (random > 80)
            return OreType.Scordite;
          else
            return OreType.Veldspar;
        case 4:
          if (random > 60)
            return OreType.Scordite;
          else
            return OreType.Veldspar;
        case 5:
          if (random > 80)
            return OreType.Pyroxeres;
          else if (random > 40)
            return OreType.Scordite;
          return OreType.Veldspar;
        case 6:
          if (random > 60)
            return OreType.Pyroxeres;
          else if (random > 20)
            return OreType.Scordite;
          return OreType.Veldspar;
      }

      return OreType.Invalid;
    }
    public static WoodType GetRandomWoodSpawnFromAreaLevel(int level)
    {
      int random = Utils.random.Next(1, 101);
      switch (level)
      {
        case 2:
          return WoodType.Laurelin;
        case 3:
          if (random > 80)
            return WoodType.Telperion;
          else
            return WoodType.Laurelin;
        case 4:
          if (random > 60)
            return WoodType.Telperion;
          else
            return WoodType.Laurelin;
        case 5:
          if (random > 80)
            return WoodType.Mallorn;
          else if (random > 40)
            return WoodType.Telperion;
          return WoodType.Laurelin;
        case 6:
          if (random > 60)
            return WoodType.Mallorn;
          else if (random > 20)
            return WoodType.Telperion;
          return WoodType.Laurelin;
      }

      return WoodType.Invalid;
    }
  }
}
