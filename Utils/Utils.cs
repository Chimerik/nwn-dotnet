using System;
using System.Collections.Generic;

namespace NWN
{
  public static class Utils
  {
    public static Random random = new Random();

    public enum Meuble

    {

      dmfi_rest = 1,

      mej_social_plc,

      mej_pc_social,

      meuble,

      mej_pc,

      mej_so

    }
    public static void LogException(Exception e)
    {
      Console.WriteLine(e.Message);
      NWScript.SendMessageToAllDMs(e.Message);
      NWScript.WriteTimestampedLogEntry(e.Message);
    }

    public static void DestroyInventory(uint oContainer)
    {
      var objectsToDestroy = new List<uint> { };
      var oObj = NWScript.GetFirstItemInInventory(oContainer);

      while (NWScript.GetIsObjectValid(oObj))
      {
        objectsToDestroy.Add(oObj);
        oObj = NWScript.GetNextItemInInventory(oContainer);
      }

      foreach (var oObject in objectsToDestroy)
      {
        NWScript.DestroyObject(oObject);
      }
    }

    public static string APSLocationToString(Location lLocation)
    {
      uint oArea = NWScript.GetAreaFromLocation(lLocation);
      Vector vPosition = NWScript.GetPositionFromLocation(lLocation);
      float fOrientation = NWScript.GetFacingFromLocation(lLocation);
      string sReturnValue = null;

      if (NWScript.GetIsObjectValid(oArea))
        sReturnValue =
            "#AREA#" + NWScript.GetTag(oArea) + "#POSITION_X#" + (vPosition.x).ToString() +
            "#POSITION_Y#" + (vPosition.y).ToString() + "#POSITION_Z#" +
            (vPosition.z).ToString() + "#ORIENTATION#" + (fOrientation).ToString() + "#END#";

      return sReturnValue;
    }
  }
}
