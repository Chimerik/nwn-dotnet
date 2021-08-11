using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

using Anvil.API;

namespace NWN.Systems.Craft.Collect
{
  public class Config
  {
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
      [Description("Planche_de_Laurelin")]
      Laurelinade = 1, // Silmarillion : capture la lumière divine dorée
      [Description("Planche_de_Telperion")]
      Telperionade = 2, // Silmarillion : capture la lumière divine argentée
      [Description("Planche_de_Mallorn")]
      Mallornade = 3,
      [Description("Planche_de_Nimloth")]
      Nimlothade = 4,
      [Description("Planche_de_Oiolaire")]
      Oiolaireade = 5,
      [Description("Planche_de_Qlipoth")]
      Qliphothade = 6,
      [Description("Planche_de_Ferochene")]
      Ferochenade = 7,
      [Description("Planche_de_Valinor")]
      Valinorade = 8,
    }
    public struct Plant
    {
      public PlantType type;
      public string name;
      public Vector2 gridEffect;
      public Plant(PlantType oreType)
      {
        this.type = oreType;
        this.name = oreType.ToDescription() ?? "";

        switch (oreType)
        {
          case PlantType.Terraria:
            gridEffect = new Vector2(1, 1);
            break;
          case PlantType.ClocheIgnee:
            gridEffect = new Vector2(0, 1);
            break;
          case PlantType.FloraisonAquatique:
            gridEffect = new Vector2(-1, 1);
            break;
          default:
            gridEffect = new Vector2(0, 0);
            break;
        }
      }
    }
    public enum PlantType
    {
      Invalid = 0,
      [Description("Terraria")]
      Terraria = 1,
      [Description("Floraison_aquatique")]
      FloraisonAquatique = 2,
      [Description("Cloche_Ignée")]
      ClocheIgnee = 3,
    }

    public static Dictionary<PlantType, Plant> plantDictionnary = new Dictionary<PlantType, Plant>()
    {
      {
        PlantType.Terraria,
        new Plant(
          oreType: PlantType.Terraria
        )
      },
      {
        PlantType.ClocheIgnee,
        new Plant(
          oreType: PlantType.ClocheIgnee
        )
      },
      {
        PlantType.FloraisonAquatique,
        new Plant(
          oreType: PlantType.FloraisonAquatique
        )
      }
    };

    public static List<ItemProperty> GetArmorProperties(NwItem craftedItem, int materialTier)
    {
      List<ItemProperty> badArmor = new List<ItemProperty>();

      switch (materialTier)
      {
        case 0:
        case 1:
          badArmor.Add(ItemProperty.ACBonus(craftedItem.BaseACValue * 3 + 7 * materialTier));

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
      craftedItem.GetObjectVariable<LocalVariableInt>("_ITEM_LEVEL").Value = materialTier;

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
