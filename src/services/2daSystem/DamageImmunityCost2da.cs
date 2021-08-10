using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class DamageImmunityTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public Entry GetDamageImmunityDataEntry(int immunityType)
    {
      return entries[immunityType];
    }
    public int GetDamageImmunityValue(int immunityType)
    {
      return entries[immunityType].value;
    }

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int tlkName = int.TryParse(twoDimEntry("Name"), out tlkName) ? tlkName : 0;
      int value = int.TryParse(twoDimEntry("Value"), out value) ? value : 0;
      entries.Add(rowIndex, new Entry(tlkName, value));
    }
    public readonly struct Entry
    {
      public readonly int tlkName;
      public readonly int value;

      public Entry(int tlkName, int value)
      {
        this.tlkName = tlkName;
        this.value = value;
      }
    }
  }

  [ServiceBinding(typeof(DamageImmunityCost2da))]
  public class DamageImmunityCost2da
  {
    public static DamageImmunityTable damamgeimmunityTable;
    public DamageImmunityCost2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      damamgeimmunityTable = twoDimArrayFactory.Get2DA<DamageImmunityTable>("iprp_immuncost");
    }
  }
}
