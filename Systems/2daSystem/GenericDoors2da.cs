using Anvil.API;
using Anvil.Services;

using NLog.Fluent;

using NWN.Core;

namespace NWN.Systems
{
  public sealed class GenericDoorsEntry : ITwoDimArrayEntry
  {
    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      string label = entry.GetString("Label");

      if (!string.IsNullOrEmpty(label) && label.Contains("supprimer"))
      {
        string model = entry.GetString("ModelName");
        ModuleSystem.Log.Info($"{model};{NWScript.ResManGetAliasFor(model, NWScript.RESTYPE_MDL)}");
      }
    }
  }

  [ServiceBinding(typeof(GenericDoors2da))]
  public class GenericDoors2da
  {
    public static readonly TwoDimArray<GenericDoorsEntry> genericDoorsTable = NwGameTables.GetTable<GenericDoorsEntry>("genericdoors.2da");
    public GenericDoors2da()
    {
      foreach (var entry in genericDoorsTable) ;
    }
  }
}
