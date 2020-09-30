using System.Collections.Generic;
using NWN.Enums;

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
      public Dictionary<MineralType, int> mineralsDictionnary = new Dictionary<MineralType, int>();
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
            this.mineralsDictionnary.Add(MineralType.Tritanium, 41);
            break;
          case OreType.Scordite:
            this.mineralsDictionnary.Add(MineralType.Tritanium, 23);
            this.mineralsDictionnary.Add(MineralType.Pyerite, 11);
            break;
          case OreType.Pyroxeres:
            this.mineralsDictionnary.Add(MineralType.Tritanium, 12);
            this.mineralsDictionnary.Add(MineralType.Pyerite, 1);
            this.mineralsDictionnary.Add(MineralType.Mexallon, 2);
            this.mineralsDictionnary.Add(MineralType.Noxcium, 1);
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
  }
}
