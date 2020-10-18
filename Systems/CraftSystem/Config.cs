using System;
using System.Collections.Generic;
using NWN.Enums.Item.Property;

namespace NWN.Systems
{
  public partial class CollectSystem
  {
    public static Dictionary<OreType, Ore> oresDictionnary = new Dictionary<OreType, Ore>();

    public partial class Ore
    {
      public OreType type;
      public string name;
      public NWN.Enums.Feat feat;
      public Dictionary<MineralType, float> mineralsDictionnary = new Dictionary<MineralType, float>();
      public Ore(OreType oreType, NWN.Enums.Feat oreFeat)
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
      oresDictionnary.Add(OreType.Veldspar, new Ore(OreType.Veldspar, NWN.Enums.Feat.VeldsparReprocessing));
      oresDictionnary.Add(OreType.Scordite, new Ore(OreType.Scordite, NWN.Enums.Feat.ScorditeReprocessing));
      oresDictionnary.Add(OreType.Pyroxeres, new Ore(OreType.Pyroxeres, NWN.Enums.Feat.PyroxeresReprocessing));
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

    public static Dictionary<BlueprintType, Blueprint> blueprintDictionnary = new Dictionary<BlueprintType, Blueprint>();
    public partial class Blueprint
    {
      public BlueprintType type;
      public string workshopTag { get; set; }
      public string craftedItemTag { get; set; }
      public int mineralsCost { get; set; }
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
            this.mineralsCost = 20000;
            this.workshopTag = "forge";
            this.craftedItemTag = "longsword";
            break;
          case BlueprintType.Fullplate:
            this.mineralsCost = 1000000;
            this.workshopTag = "forge";
            this.craftedItemTag = "fullplate";
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

    public static Dictionary<Tuple<MineralType, ItemSystem.ItemCategory>, List<ItemProperty>> itemPropertiesDictionnary = new Dictionary<Tuple<MineralType, ItemSystem.ItemCategory>, List<ItemProperty>>();
    public static Dictionary<int, List<ItemProperty>> test = new Dictionary<int, List<ItemProperty>>();
    //public static Dictionary<BaseItem, List<ItemProperty>> itemPropertiesDictionnary = new Dictionary<BaseItem, List<ItemProperty>>();

    public static void initiateCraftItemProperties()
    { 
      List<ItemProperty> itemPropertyList = new List<ItemProperty>();
      //NWNX.Enum.ItemPropertyUnpacked prout;
      
      // TRITANIUM
      itemPropertyList.Add(NWScript.ItemPropertyDamageVulnerability(Enums.Item.Property.DamageType.Fire, DamageVulnerability.FiftyPERCENT));
      itemPropertyList.Add(NWScript.ItemPropertyDamageVulnerability(Enums.Item.Property.DamageType.Cold, DamageVulnerability.FiftyPERCENT));
      itemPropertyList.Add(NWScript.ItemPropertyDamageVulnerability(Enums.Item.Property.DamageType.Electrical, DamageVulnerability.FiftyPERCENT));
      itemPropertyList.Add(NWScript.ItemPropertyWeightReduction(ReducedWeight.FourtyPercent));
      
      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.OneHandedMeleeWeapon), itemPropertyList);
      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.TwoHandedMeleeWeapon), itemPropertyList);
      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.Armor), itemPropertyList);
      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.Shield), itemPropertyList);

      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.OneHandedMeleeWeapon), itemPropertyList);
      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.TwoHandedMeleeWeapon), itemPropertyList);
      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.Armor), itemPropertyList);
      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Tritanium, ItemSystem.ItemCategory.Shield), itemPropertyList);

      itemPropertyList.Clear();

      // PYERITE
      itemPropertyList.Add(NWScript.ItemPropertyDamageVulnerability(Enums.Item.Property.DamageType.Fire, DamageVulnerability.TwentyFivePERCENT));
      itemPropertyList.Add(NWScript.ItemPropertyWeightIncrease(WeightIncrease.TenPounds));
      itemPropertyList.Add(NWScript.ItemPropertyAttackBonusVsRace(Enums.Item.Property.RacialType.HumanoidReptilian, 1));
      itemPropertyList.Add(NWScript.ItemPropertyAttackBonusVsRace(Enums.Item.Property.RacialType.HumanoidGoblinoid, 1));
      
      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.OneHandedMeleeWeapon), itemPropertyList);
      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.TwoHandedMeleeWeapon), itemPropertyList);

      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.OneHandedMeleeWeapon), itemPropertyList);
      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.TwoHandedMeleeWeapon), itemPropertyList);

      itemPropertyList.Clear();

      itemPropertyList.Add(NWScript.ItemPropertyDamageVulnerability(Enums.Item.Property.DamageType.Fire, DamageVulnerability.TwentyFivePERCENT));
      itemPropertyList.Add(NWScript.ItemPropertyWeightIncrease(WeightIncrease.TenPounds));
      itemPropertyList.Add(NWScript.ItemPropertyACBonusVsRace(Enums.Item.Property.RacialType.HumanoidGoblinoid, 1));
      itemPropertyList.Add(NWScript.ItemPropertyACBonusVsRace(Enums.Item.Property.RacialType.HumanoidReptilian, 1));

      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.Armor), itemPropertyList);
      itemPropertiesDictionnary.Add(new Tuple<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.Shield), itemPropertyList);

      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.Shield), itemPropertyList);
      test.Add(HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.Armor), itemPropertyList);

      itemPropertyList.Clear();
    }
  }
}
