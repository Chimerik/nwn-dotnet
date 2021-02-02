using NWN.Core;

namespace NWN
{
  public static class AreaUtils
  {
    public static uint GetObjectInAreaByTag(uint oArea, string sTag)
    {
      var oObj = NWScript.GetFirstObjectInArea(oArea);

      return NWScript.GetTag(oObj) == sTag ? oObj : NWScript.GetNearestObjectByTag(sTag, oObj);
    }
  }
}
