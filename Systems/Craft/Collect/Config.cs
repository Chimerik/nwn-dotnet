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
    public static Dictionary<WoodType, Wood> woodDictionnary = new Dictionary<WoodType, Wood>()
    {
      {
        WoodType.Laurelin,
        new Wood(
          oreType: WoodType.Laurelin,
          oreFeat: Feat.LaurelinReprocessing
        )
      },
      {
        WoodType.Telperion,
        new Wood(
          oreType: WoodType.Telperion,
          oreFeat: Feat.TelperionReprocessing
        )
      },
      {
        WoodType.Mallorn,
        new Wood(
          oreType: WoodType.Mallorn,
          oreFeat: Feat.MallornReprocessing
        )
      }
      };
    public static Dictionary<PlankType, Plank> plankDictionnary = new Dictionary<PlankType, Plank>
    {
      { PlankType.Laurelinade, new Plank(PlankType.Laurelinade) },
      { PlankType.Telperionade, new Plank(PlankType.Telperionade) },
      { PlankType.Mallornade, new Plank(PlankType.Mallornade) },
    };
    public struct Pelt
    {
      public PeltType type;
      public string name;
      public Feat feat;
      public float leathers;
      public LeatherType refinedType;
      public Pelt(PeltType oreType, Feat oreFeat)
      {
        this.type = oreType;
        this.name = Enum.GetName(typeof(PeltType), oreType) ?? "";
        this.feat = oreFeat;

        switch(oreType)
        {
          case PeltType.MauvaisePeau:
            leathers = 41.500f;
            refinedType = LeatherType.MauvaisCuir;
            break;
          case PeltType.PeauCommune:
            leathers = 34.567f;
            refinedType = LeatherType.CuirCommun;
            break;
          case PeltType.PeauNormale:
            leathers = 15.000f;
            refinedType = LeatherType.CuirNormal;
            break;
          default:
            leathers = 0.0f;
            refinedType = LeatherType.Invalid;
            break;
        }
      }
    }
    public struct Leather
    {
      public LeatherType type;
      public string name;

      public Leather(LeatherType type)
      {
        this.type = type;
        this.name = Enum.GetName(typeof(LeatherType), type) ?? "";
      }
    }
    public enum PeltType
    {
      Invalid = 0,
      MauvaisePeau = 1, 
      PeauCommune = 2, 
      PeauNormale = 3,
      PeauPeuCommune = 4,
      PeauRare = 5,
      PeauMagique = 6,
      PeauEpique = 7,
      PeauLegendaire = 8,
    }
    public enum LeatherType
    {
      Invalid = 0,
      MauvaisCuir = 1,
      CuirCommun = 2,
      CuirNormal = 3,
      CuirPeuCommun = 4,
      CuirRare = 5,
      CuirMagique = 6,
      CuirEpique = 7,
      CuirLegendaire = 8,
    }
    public static Dictionary<PeltType, Pelt> peltDictionnary = new Dictionary<PeltType, Pelt>()
    {
      {
        PeltType.MauvaisePeau,
        new Pelt(
          oreType: PeltType.MauvaisePeau,
          oreFeat: Feat.BadPeltReprocessing
        )
      },
      {
        PeltType.PeauCommune,
        new Pelt(
          oreType: PeltType.PeauCommune,
          oreFeat: Feat.CommonPeltReprocessing
        )
      },
      {
        PeltType.PeauNormale,
        new Pelt(
          oreType: PeltType.PeauNormale,
          oreFeat: Feat.NormalPeltReprocessing
        )
      }
      };
    public static Dictionary<LeatherType, Leather> leatherDictionnary = new Dictionary<LeatherType, Leather>
    {
      { LeatherType.MauvaisCuir, new Leather(LeatherType.MauvaisCuir) },
      { LeatherType.CuirCommun, new Leather(LeatherType.CuirCommun) },
      { LeatherType.CuirNormal, new Leather(LeatherType.CuirNormal) },
    };
    public struct Wood
    {
      public WoodType type;
      public string name;
      public Feat feat;
      public float planks;
      public PlankType refinedType;
      public Wood(WoodType oreType, Feat oreFeat)
      {
        this.type = oreType;
        this.name = Enum.GetName(typeof(WoodType), oreType) ?? "";
        this.feat = oreFeat;

        switch (oreType)
        {
          case WoodType.Laurelin:
            planks = 41.500f;
            refinedType = PlankType.Laurelinade;
            break;
          case WoodType.Telperion:
            planks = 34.567f;
            refinedType = PlankType.Telperionade;
            break;
          case WoodType.Mallorn:
            planks = 15.000f;
            refinedType = PlankType.Mallornade;
            break;
          default:
            planks = 0.0f;
            refinedType = PlankType.Invalid;
            break;
        }
      }
    }
    public struct Plank
    {
      public PlankType type;
      public string name;

      public Plank(PlankType type)
      {
        this.type = type;
        this.name = Enum.GetName(typeof(PlankType), type) ?? "";
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
      Laurelinade = 1, // Silmarillion : capture la lumière divine dorée
      Telperionade = 2, // Silmarillion : capture la lumière divine argentée
      Mallornade = 3,
      Nimlothade = 4,
      Oiolaireade = 5,
      Qliphothade = 6,
      Ferochenade = 7,
      Valinorade = 8,
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
        case ItemCategory.RangedWeapon: return GetPyeriteOneHandedMeleeWeaponProperties();
        case ItemCategory.Ammunition: return GetPyeriteAmmunitionProperties();
      }

      return new ItemProperty[]
      { 
          NWScript.ItemPropertyVisualEffect(NWScript.VFX_NONE)
      };
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
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, NWScript.IP_CONST_DAMAGETYPE_PIERCING, NWScript.DAMAGE_BONUS_1),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, NWScript.IP_CONST_DAMAGETYPE_PIERCING, NWScript.DAMAGE_BONUS_1)
        };
      }
    public static ItemProperty[] GetPyeriteAmmunitionProperties()
    {
      return new ItemProperty[]
      {
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_COLD, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL, NWScript.IP_CONST_DAMAGEVULNERABILITY_25_PERCENT),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, 1),
          NWScript.ItemPropertyAttackBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, 1),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, NWScript.IP_CONST_DAMAGETYPE_PIERCING, NWScript.DAMAGE_BONUS_1),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, NWScript.IP_CONST_DAMAGETYPE_PIERCING, NWScript.DAMAGE_BONUS_1)
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
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_GOBLINOID, NWScript.IP_CONST_DAMAGETYPE_PIERCING, NWScript.DAMAGE_BONUS_2),
          NWScript.ItemPropertyDamageBonusVsRace(NWScript.RACIAL_TYPE_HUMANOID_REPTILIAN, NWScript.IP_CONST_DAMAGETYPE_PIERCING, NWScript.DAMAGE_BONUS_2)
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
    public static PeltType GetRandomPeltSpawnFromAreaLevel(int level)
    {
      int random = Utils.random.Next(1, 101);
      switch (level)
      {
        case 2:
          return PeltType.MauvaisePeau;
        case 3:
          if (random > 80)
            return PeltType.PeauCommune;
          else
            return PeltType.MauvaisePeau;
        case 4:
          if (random > 60)
            return PeltType.PeauCommune;
          else
            return PeltType.MauvaisePeau;
        case 5:
          if (random > 80)
            return PeltType.PeauNormale;
          else if (random > 40)
            return PeltType.PeauCommune;
          return PeltType.MauvaisePeau;
        case 6:
          if (random > 60)
            return PeltType.PeauNormale;
          else if (random > 20)
            return PeltType.PeauCommune;
          return PeltType.MauvaisePeau;
      }

      return PeltType.Invalid;
    }
  }
}
