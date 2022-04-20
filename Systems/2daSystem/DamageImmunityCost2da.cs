using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class DamageImmunityEntry : ITwoDimArrayEntry
  {
    public StrRef tlkName { get; private set; }
    public int value { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      tlkName = entry.GetStrRef("Name").Value;
      value = entry.GetInt("Value").GetValueOrDefault(0);
    }
  }

  [ServiceBinding(typeof(DamageImmunityCost2da))]
  public class DamageImmunityCost2da
  {
    public static readonly TwoDimArray<DamageImmunityEntry> damageImmunityTable = new("iprp_immuncost.2da");
    public DamageImmunityCost2da()
    {
    }
  }
}
