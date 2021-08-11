using System.Collections.Generic;
using System.ComponentModel;

using Anvil.API;

namespace Utils
{
  public static class WoodConfig
  {
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

    public static Dictionary<WoodType, Wood> woodDictionnary = new Dictionary<WoodType, Wood>()
    {
      {
        WoodType.Laurelin,
        new Wood(
          oreType: WoodType.Laurelin,
          oreFeat: CustomFeatList.LaurelinReprocessing
        )
      },
      {
        WoodType.Telperion,
        new Wood(
          oreType: WoodType.Telperion,
          oreFeat: CustomFeatList.TelperionReprocessing
        )
      },
      {
        WoodType.Mallorn,
        new Wood(
          oreType: WoodType.Mallorn,
          oreFeat: CustomFeatList.MallornReprocessing
        )
      }
    };

    public static Dictionary<PlankType, Plank> plankDictionnary = new Dictionary<PlankType, Plank>
    {
      { PlankType.Laurelinade, new Plank(PlankType.Laurelinade) },
      { PlankType.Telperionade, new Plank(PlankType.Telperionade) },
      { PlankType.Mallornade, new Plank(PlankType.Mallornade) },
    };
    public static WoodType GetRandomWoodSpawnFromAreaLevel(int level)
    {
      int random = MiscUtils.random.Next(1, 101);
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
