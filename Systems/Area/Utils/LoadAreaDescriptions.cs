using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class AreaUtils
  {
    public static async void LoadAreaDescription(NwArea area)
    {
      string description = await StringUtils.DownloadGoogleDocFromName(area.Name);

      if (string.IsNullOrEmpty(description))
        return;

      if (!AreaSystem.areaDescriptions.TryAdd(area.Name, description))
        AreaSystem.areaDescriptions[area.Name] = description;
    }
  }
}
