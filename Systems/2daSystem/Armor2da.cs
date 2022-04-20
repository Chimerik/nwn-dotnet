using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ArmorEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }
    public int cost { get; private set; }
    public string workshop { get; private set; }
    public string craftResRef { get; private set; }
    public int maxDex { get; private set; }
    public int ACPenalty { get; private set; }
    public int arcaneFailure { get; private set; }
    public int craftLearnable { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      cost = entry.GetInt("COST").GetValueOrDefault(0);
      name = entry.GetStrRef("NAME").GetValueOrDefault().ToString();
      workshop = entry.GetString("WORKSHOP");
      craftResRef = entry.GetString("CRAFTRESREF");
      maxDex = entry.GetInt("DEXBONUS").GetValueOrDefault(0);
      ACPenalty = entry.GetInt("ACCHECK").GetValueOrDefault(0);
      arcaneFailure = entry.GetInt("ARCANEFAILURE").GetValueOrDefault(0);
      craftLearnable = entry.GetInt("ACBONUS%").GetValueOrDefault(-1);
    }
  }

  [ServiceBinding(typeof(Armor2da))]
  public class Armor2da
  {
    public static readonly TwoDimArray<ArmorEntry> armorTable = new("armor.2da");
    public Armor2da()
    {
      
    }

    public static int GetCost(int baseACV)
    {
      return armorTable[baseACV].cost;
    }
    public static string GetWorkshop(int baseACV)
    {
      return armorTable[baseACV].workshop;
    }
  }
}
