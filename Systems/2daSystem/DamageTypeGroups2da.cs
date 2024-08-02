using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class DamageTypeGroupsEntry : ITwoDimArrayEntry
  {
    public string damageName { get; private set; }
    public Color color { get; private set; }
    public DamageType damageType { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      if (RowIndex < 1)
        return;

      damageName = entry.GetString("Label");
      damageType = (DamageType)entry.GetInt("DamageType").Value;

      var temp = new byte[4] { (byte)entry.GetInt("ColorR").GetValueOrDefault(0), (byte)entry.GetInt("ColorG").GetValueOrDefault(0), (byte)entry.GetInt("ColorG").GetValueOrDefault(0), 0 };
      color = new Color(temp[0], temp[1], temp[2], temp[3]);

      ModuleSystem.Log.Info($"{color.Red} - {color.Green} - {color.Blue}");
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
