using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class DamageTypeGroupsEntry : ITwoDimArrayEntry
  {
    public StrRef tlkName { get; private set; }
    public Color color { get; private set; }
    public DamageType damageType { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      if (RowIndex < 1)
        return;

      tlkName = entry.GetStrRef("FeedbackStrref").Value;
      damageType = (DamageType)entry.GetInt("DamageType").Value;
      color = new Color(entry.GetInt("ColorR").GetValueOrDefault(0), entry.GetInt("ColorG").GetValueOrDefault(0), entry.GetInt("ColorB").GetValueOrDefault(0));
    }
  }

  [ServiceBinding(typeof(DamageTypeGroups2da))]
  public class DamageTypeGroups2da
  {
    public static readonly TwoDimArray<DamageTypeGroupsEntry> damageTypeGroupsTable = NwGameTables.GetTable<DamageTypeGroupsEntry>("damagetypegroups.2da");
    public DamageTypeGroups2da()
    {
      foreach (var entry in damageTypeGroupsTable) ;
        //damageStringConf.Add(entry.damageType, entry);
    }
  }
}
