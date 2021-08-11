using System.Collections.Generic;

using Anvil.API;

namespace Utils
{
  public static class OreConfig
  {
    public static Dictionary<OreType, Ore> oresDictionnary = new Dictionary<OreType, Ore>
    {
      {
        OreType.Veldspar,
        new Ore(
          oreType: OreType.Veldspar,
          oreFeat: CustomFeatList.VeldsparReprocessing,
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
          oreFeat: CustomFeatList.ScorditeReprocessing,
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
          oreFeat: CustomFeatList.PyroxeresReprocessing,
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
    public static OreType GetRandomOreSpawnFromAreaLevel(int level)
    {
      int random = MiscUtils.random.Next(1, 101);
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
  }
}
