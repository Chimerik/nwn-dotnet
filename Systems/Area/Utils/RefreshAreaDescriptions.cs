using Anvil.API;

namespace NWN.Systems
{
  public static partial class AreaUtils
  {
    public static void RefreshAreaDescriptions()
    {
      AreaSystem.areaDescriptionsToDownload.Clear();

      foreach(NwArea area in NwModule.Instance.Areas)
        AreaSystem.areaDescriptionsToDownload.Add(area.Name);

      foreach(string areaName in AreaSystem.areaDescriptionsToDownload)
        LoadAreaDescription(areaName);
    }
  }
}
