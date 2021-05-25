using System;
using System.Collections.Generic;
using System.ComponentModel;
using static NWN.Systems.ItemUtils;
using NWN.API;
using NWN.API.Constants;

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
          oreFeat: CustomFeats.VeldsparReprocessing,
          mineralsDictionnary: new Dictionary<MineralType, float>
          {
            { MineralType.Tritanium, 2.075f }
          }
        )
      },
      {
        OreType.Scordite,
        new Ore(
          oreType: OreType.Scordite,
          oreFeat: CustomFeats.ScorditeReprocessing,
          mineralsDictionnary: new Dictionary<MineralType, float>
          {
            { MineralType.Tritanium, 1.15335f },
            { MineralType.Pyerite, 0.57665f}
          }
        )
      },
      {
        OreType.Pyroxeres,
        new Ore(
          oreType: OreType.Pyroxeres,
          oreFeat: CustomFeats.PyroxeresReprocessing,
          mineralsDictionnary: new Dictionary<MineralType, float>
          {
            { MineralType.Tritanium, 0.585f },
            { MineralType.Pyerite, 0.04165f },
            { MineralType.Mexallon,  0.08335f },
            { MineralType.Noxcium, 0.00835f }
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
        this.name = oreType.ToDescription() ?? "";
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
        this.name = type.ToDescription() ?? "";
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
          oreFeat: CustomFeats.LaurelinReprocessing
        )
      },
      {
        WoodType.Telperion,
        new Wood(
          oreType: WoodType.Telperion,
          oreFeat: CustomFeats.TelperionReprocessing
        )
      },
      {
        WoodType.Mallorn,
        new Wood(
          oreType: WoodType.Mallorn,
          oreFeat: CustomFeats.MallornReprocessing
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
        this.name = oreType.ToDescription() ?? "";
        this.feat = oreFeat;

        switch (oreType)
        {
          case PeltType.MauvaisePeau:
            leathers = 2.075f;
            refinedType = LeatherType.MauvaisCuir;
            break;
          case PeltType.PeauCommune:
            leathers = 1.72835f;
            refinedType = LeatherType.CuirCommun;
            break;
          case PeltType.PeauNormale:
            leathers = 0.75f;
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
        this.name = type.ToDescription() ?? "";
      }
    }
    public enum PeltType
    {
      Invalid = 0,
      [Description("Peau_de_mauvaise_qualite")]
      MauvaisePeau = 1,
      [Description("Peau_commune")]
      PeauCommune = 2,
      [Description("Peau_normale")]
      PeauNormale = 3,
      [Description("Peau_inhabituelle")]
      PeauPeuCommune = 4,
      [Description("Peau_rare")]
      PeauRare = 5,
      [Description("Peau_magique")]
      PeauMagique = 6,
      [Description("Peau_epique")]
      PeauEpique = 7,
      [Description("Peau_legendaire")]
      PeauLegendaire = 8,
    }
    public enum LeatherType
    {
      Invalid = 0,
      [Description("Cuir_de_mauvaise_qualite")]
      MauvaisCuir = 1,
      [Description("Cuir_commun")]
      CuirCommun = 2,
      [Description("Cuir_normal")]
      CuirNormal = 3,
      [Description("Cuir_peu_commun")]
      CuirPeuCommun = 4,
      [Description("Cuir_rare")]
      CuirRare = 5,
      [Description("Cuir_magique")]
      CuirMagique = 6,
      [Description("Cuir_epique")]
      CuirEpique = 7,
      [Description("Cuir_legendaire")]
      CuirLegendaire = 8,
    }
    public static Dictionary<PeltType, Pelt> peltDictionnary = new Dictionary<PeltType, Pelt>()
    {
      {
        PeltType.MauvaisePeau,
        new Pelt(
          oreType: PeltType.MauvaisePeau,
          oreFeat: CustomFeats.BadPeltReprocessing
        )
      },
      {
        PeltType.PeauCommune,
        new Pelt(
          oreType: PeltType.PeauCommune,
          oreFeat: CustomFeats.CommonPeltReprocessing
        )
      },
      {
        PeltType.PeauNormale,
        new Pelt(
          oreType: PeltType.PeauNormale,
          oreFeat: CustomFeats.NormalPeltReprocessing
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
        this.name = oreType.ToDescription() ?? "";
        this.feat = oreFeat;

        switch (oreType)
        {
          case WoodType.Laurelin:
            planks = 2.075f;
            refinedType = PlankType.Laurelinade;
            break;
          case WoodType.Telperion:
            planks = 1.72835f;
            refinedType = PlankType.Telperionade;
            break;
          case WoodType.Mallorn:
            planks = 0.75f;
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
        this.name = type.ToDescription() ?? "";
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
      [Description("Bois_de_Laurelin")]
      Laurelinade = 1, // Silmarillion : capture la lumière divine dorée
      [Description("Bois_de_Telperion")]
      Telperionade = 2, // Silmarillion : capture la lumière divine argentée
      [Description("Bois_de_Mallorn")]
      Mallornade = 3,
      [Description("Bois_de_Nimloth")]
      Nimlothade = 4,
      [Description("Bois_de_Oiolaire")]
      Oiolaireade = 5,
      [Description("Bois_de_Qlipoth")]
      Qliphothade = 6,
      [Description("Bois_de_Ferochene")]
      Ferochenade = 7,
      [Description("Bois_de_Valinor")]
      Valinorade = 8,
    }
    public static ItemProperty[] GetBadItemProperties(ItemCategory itemCategory, NwItem craftedItem)
    {
      craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value = GetBaseItemCost(craftedItem) * 25;
      craftedItem.GetLocalVariable<int>("_DURABILITY").Value = GetBaseItemCost(craftedItem) * 25;

      /*switch (itemCategory)
      {
        case ItemCategory.OneHandedMeleeWeapon: return GetBadOneHandedMeleeWeaponProperties();
        case ItemCategory.TwoHandedMeleeWeapon: return GetBadTwoHandedMeleeWeaponProperties();
        case ItemCategory.Armor: return GetBadArmorProperties();
        case ItemCategory.Shield: return GetBadShieldProperties();
        case ItemCategory.CraftTool: return GetBadToolProperties(craftedItem);
        case ItemCategory.RangedWeapon: return GetBadRangedWeaponProperties();
        case ItemCategory.Clothes: return GetBadClothesProperties();
      }*/

      return new API.ItemProperty[]
      {
          API.ItemProperty.Quality(IPQuality.VeryPoor)
      };
    }
    public static API.ItemProperty[] GetBadOneHandedMeleeWeaponProperties()
    {
      return new API.ItemProperty[]
      {
          API.ItemProperty.AttackPenalty(2),
          API.ItemProperty.DamagePenalty(2),
          API.ItemProperty.DamageVulnerability(IPDamageType.Slashing, IPDamageVulnerabilityType.Vulnerable10Pct),
          API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus30Lbs),
      };
    }
    public static API.ItemProperty[] GetBadTwoHandedMeleeWeaponProperties()
    {
      return new API.ItemProperty[]
      {
          API.ItemProperty.AttackPenalty(1),
          API.ItemProperty.DamagePenalty(1),
          API.ItemProperty.DamageVulnerability(IPDamageType.Slashing, IPDamageVulnerabilityType.Vulnerable5Pct),
          API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus30Lbs),
      };
    }
    public static API.ItemProperty[] GetBadRangedWeaponProperties()
    {
      return new API.ItemProperty[]
      {
          API.ItemProperty.AttackPenalty(1),
          API.ItemProperty.DamageVulnerability(IPDamageType.Slashing, IPDamageVulnerabilityType.Vulnerable5Pct),
          API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus30Lbs),
      };
    }
    public static API.ItemProperty[] GetBadArmorProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.DamageVulnerability(IPDamageType.Bludgeoning, IPDamageVulnerabilityType.Vulnerable10Pct),
        API.ItemProperty.DecreaseAC(IPACModifierType.Armor, 2),
        API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus30Lbs),
      };
    }
    public static API.ItemProperty[] GetBadShieldProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.DamageVulnerability(IPDamageType.Piercing, IPDamageVulnerabilityType.Vulnerable10Pct),
        API.ItemProperty.DecreaseAC(IPACModifierType.Armor, 2),
        API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus30Lbs),
      };
    }
    public static API.ItemProperty[] GetBadToolProperties(NwItem craftedItem)
    {
      craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value = 5;
      craftedItem.GetLocalVariable<int>("_DURABILITY").Value = 5;

      return new API.ItemProperty[]
      {
        API.ItemProperty.DamageVulnerability(IPDamageType.Piercing, IPDamageVulnerabilityType.Vulnerable10Pct),
        API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus15Lbs),
      };
    }
    public static API.ItemProperty[] GetBadClothesProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.DamageVulnerability(IPDamageType.Bludgeoning, IPDamageVulnerabilityType.Vulnerable10Pct),
        API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus30Lbs),
        API.ItemProperty.DecreaseAC(IPACModifierType.Armor, 2),
      };
    }
    public static API.ItemProperty[] GetTritaniumItemProperties(NwItem craftedItem = null)
    {
      if (craftedItem != null)
      {
        craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value = GetBaseItemCost(craftedItem) * 100;
        craftedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 1;
      }

      return new API.ItemProperty[]
      {
          API.ItemProperty.Quality(IPQuality.Poor)
      };
    }
    public static API.ItemProperty[] GetPyeriteItemProperties(ItemCategory itemCategory, NwItem craftedItem = null)
    {
      if (craftedItem != null)
      {
        craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value = GetBaseItemCost(craftedItem) * 200;
        craftedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 2;
      }

      switch (itemCategory)
      {
        case ItemCategory.OneHandedMeleeWeapon: return GetPyeriteOneHandedMeleeWeaponProperties();
        case ItemCategory.TwoHandedMeleeWeapon: return GetPyeriteTwoHandedMeleeWeaponProperties();
        case ItemCategory.Armor: return GetPyeriteArmorProperties();
        case ItemCategory.Shield: return GetPyeriteShieldProperties();
        case ItemCategory.CraftTool: return GetPyeriteToolProperties(craftedItem);
        case ItemCategory.RangedWeapon: return GetPyeriteOneHandedMeleeWeaponProperties();
        case ItemCategory.Ammunition: return GetPyeriteAmmunitionProperties();
        case ItemCategory.Clothes: return GetPyeriteArmorProperties();
      }

      return new API.ItemProperty[]
      {
          API.ItemProperty.Quality(IPQuality.Unknown)
    };
    }
    public static API.ItemProperty[] GetPyeriteOneHandedMeleeWeaponProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidGoblinoid, 1),
        API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidReptilian, 1),
        API.ItemProperty.DamageBonusVsRace(IPRacialType.HumanoidGoblinoid, IPDamageType.Piercing, IPDamageBonus.Plus1),
        API.ItemProperty.DamageBonusVsRace(IPRacialType.HumanoidReptilian, IPDamageType.Piercing, IPDamageBonus.Plus1),
      };
    }
    public static API.ItemProperty[] GetPyeriteAmmunitionProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidGoblinoid, 1),
        API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidReptilian, 1),
        API.ItemProperty.DamageBonusVsRace(IPRacialType.HumanoidGoblinoid, IPDamageType.Piercing, IPDamageBonus.Plus1),
        API.ItemProperty.DamageBonusVsRace(IPRacialType.HumanoidReptilian, IPDamageType.Piercing, IPDamageBonus.Plus1),
      };
    }
    public static API.ItemProperty[] GetPyeriteTwoHandedMeleeWeaponProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidGoblinoid, 2),
        API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidReptilian, 2),
        API.ItemProperty.DamageBonusVsRace(IPRacialType.HumanoidGoblinoid, IPDamageType.Piercing, IPDamageBonus.Plus2),
        API.ItemProperty.DamageBonusVsRace(IPRacialType.HumanoidReptilian, IPDamageType.Piercing, IPDamageBonus.Plus2),
      };
    }
    public static API.ItemProperty[] GetPyeriteArmorProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.ACBonusVsDmgType(IPDamageType.Bludgeoning, 1),
      };
    }
    public static API.ItemProperty[] GetPyeriteShieldProperties()
    {
      return new API.ItemProperty[]
      {
        API.ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 1),
      };
    }
    public static ItemProperty[] GetPyeriteToolProperties(NwItem craftedItem)
    {
      craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value = 25;
      craftedItem.GetLocalVariable<int>("_ITEM_LEVEL").Value = 1;

      return new API.ItemProperty[]
      {
          API.ItemProperty.WeightIncrease(IPWeightIncrease.Plus10Lbs),
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
    public static string GetRandomPeltSpawnFromAreaLevel(int level)
    {
      int random = Utils.random.Next(1, 101);
      switch (level)
      {
        case 2:
          if (random > 80)
            return System.commonPelts[Utils.random.Next(0, System.commonPelts.Length)];
          else
            return System.badPelts[Utils.random.Next(0, System.badPelts.Length)];
        case 3:
          if (random > 60)
            return System.commonPelts[Utils.random.Next(0, System.commonPelts.Length)];
          else
            return System.badPelts[Utils.random.Next(0, System.badPelts.Length)];
        case 4:
          if (random > 80)
            return System.normalPelts[Utils.random.Next(0, System.normalPelts.Length)];
          else if (random > 40)
            return System.commonPelts[Utils.random.Next(0, System.commonPelts.Length)];
          return System.badPelts[Utils.random.Next(0, System.badPelts.Length)];
        case 5:
          if (random > 60)
            return System.normalPelts[Utils.random.Next(0, System.normalPelts.Length)];
          else if (random > 20)
            return System.commonPelts[Utils.random.Next(0, System.commonPelts.Length)];
          return System.badPelts[Utils.random.Next(0, System.badPelts.Length)];
        case 6:
          if (random > 40)
            return System.normalPelts[Utils.random.Next(0, System.normalPelts.Length)];

          return System.commonPelts[Utils.random.Next(0, System.commonPelts.Length)];
      }

      return "";
    }
    public static PeltType GetPeltTypeFromItemTag(string itemTag)
    {
      if (Array.FindIndex(System.badPelts, x => x == itemTag) > -1)
        return PeltType.MauvaisePeau;
      else if (Array.FindIndex(System.commonPelts, x => x == itemTag) > -1)
        return PeltType.PeauCommune;
      else if (Array.FindIndex(System.normalPelts, x => x == itemTag) > -1)
        return PeltType.PeauNormale;

      return PeltType.Invalid;
    }


  }
}
