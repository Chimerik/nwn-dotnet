using Anvil.API;
using Anvil.Services;

using NLog.Fluent;
using NWN.Core;

namespace NWN.Systems
{
  public sealed class SkyboxEntry : ITwoDimArrayEntry
  {
    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      string label = entry.GetString("LABEL");

      if (!string.IsNullOrEmpty(label) && label.Contains("supprimer"))
      {
        string dawn = entry.GetString("DAWN");
        string day = entry.GetString("DAY");
        string dusk = entry.GetString("DUSK");
        string night = entry.GetString("NIGHT");

        ModuleSystem.Log.Info($"{dawn};{NWScript.ResManGetAliasFor(dawn, NWScript.RESTYPE_MDL)}");
        ModuleSystem.Log.Info($"{day};{NWScript.ResManGetAliasFor(day, NWScript.RESTYPE_MDL)}");
        ModuleSystem.Log.Info($"{dusk};{NWScript.ResManGetAliasFor(dusk, NWScript.RESTYPE_MDL)}");
        ModuleSystem.Log.Info($"{night};{NWScript.ResManGetAliasFor(night, NWScript.RESTYPE_MDL)}");
      }
    }
  }

  [ServiceBinding(typeof(Skybox2da))]
  public class Skybox2da
  {
    public static readonly TwoDimArray<SkyboxEntry> skyboxTable = NwGameTables.GetTable<SkyboxEntry>("skyboxes.2da");
    public Skybox2da()
    {
      //foreach (var entry in skyboxTable) ;
    }
  }
}
