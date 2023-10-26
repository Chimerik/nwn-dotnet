namespace NWN.Systems
{
  public static partial class AreaUtils
  {
    public static async void LoadAreaDescription(string areaName)
    {
      string description = await StringUtils.DownloadGoogleDocFromName(areaName);

      if (string.IsNullOrEmpty(description))
        return;

      if (!AreaSystem.areaDescriptions.TryAdd(areaName, description))
        AreaSystem.areaDescriptions[areaName] = description;
    }
  }
}
