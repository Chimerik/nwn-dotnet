using Anvil.API;

namespace NWN
{
  public static partial class AreaUtils
  {
    public static void RefreshAreaDescriptions()
    {
      foreach(NwArea area in NwModule.Instance.Areas)
        LoadAreaDescription(area);
    }
  }
}
