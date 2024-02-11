using System;

namespace NWN.Systems
{
  public static partial class AreaUtils
  {
    public static async void LoadAreaDescription(string areaName)
    {
      try
      {
        string description = await StringUtils.DownloadGoogleDocFromName(areaName);

        if (string.IsNullOrEmpty(description))
          return;

        if (!AreaSystem.areaDescriptions.TryAdd(areaName, description))
          AreaSystem.areaDescriptions[areaName] = description;
      }
      catch(Exception e)
      {
        LogUtils.LogMessage($"Could not load {areaName} description\n{e.Message}\n{e.InnerException}\n{e.StackTrace}", LogUtils.LogType.AreaManagement);
      }
    }
  }
}
