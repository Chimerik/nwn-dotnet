using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NWN.Enums;

namespace NWN.Systems
{
  public partial class CollectSystem
  {
    public static Dictionary<string, Ore> oresDictionnary = new Dictionary<string, Ore>();

    public partial class Ore
    {
      public string name;
      public Feat feat;
      public Dictionary<string, int> mineralsDictionnary = new Dictionary<string, int>();
      public Ore(string oreName, Feat oreFeat)
      {
        this.name = oreName;
        this.feat = oreFeat;
        this.InitiateOreRefinementYield();
      }

      public void InitiateOreRefinementYield()
      {
        switch (this.name)
        {
          case "Veldspar":
            this.mineralsDictionnary.Add("Tritanium", 41);
            break;
          case "Scordite":
            this.mineralsDictionnary.Add("Tritanium", 23);
            this.mineralsDictionnary.Add("Pyerite", 11);
            break;
          case "Pyroxeres":
            this.mineralsDictionnary.Add("Tritanium", 12);
            this.mineralsDictionnary.Add("Pyerite", 1);
            this.mineralsDictionnary.Add("Mexallon", 2);
            this.mineralsDictionnary.Add("Nocxium", 1);
            break;
        }
      }
    }

    public static void InitiateOres()
    {
      oresDictionnary.Add("Veldspar", new Ore("Veldspar", Feat.VeldsparReprocessing));
      oresDictionnary.Add("Scordite", new Ore("Scordite", Feat.ScorditeReprocessing));
      oresDictionnary.Add("Pyroxeres", new Ore("Pyroxeres", Feat.PyroxeresReprocessing));
    }

    /*public enum Ore
    {
      Veldspar = 1,
      Scordite = 2,
      Pyroxeres = 3,
    }*/
  }
}
