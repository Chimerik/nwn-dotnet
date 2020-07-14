using System;
using System.Collections.Generic;

namespace NWN
{
  public static class Utils
  {
    public static Random random = new Random();

    public enum Meuble // je le garde uniquement pour avoir un exemple à disposition de comment gérer les enum et les TryParse. Nous on utilisera des tag plus parlant pour nos meubles

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

    public static string LocationToString(Location l)
    {
      uint area = NWScript.GetAreaFromLocation(l);
      Vector pos = NWScript.GetPositionFromLocation(l);
      float facing = NWScript.GetFacingFromLocation(l);

      return "#TAG#" + NWScript.GetTag(area) + "#RESREF#" + NWScript.GetResRef(area) +
              "#X#" + NWScript.FloatToString(pos.x, 5, 2) +
              "#Y#" + NWScript.FloatToString(pos.y, 5, 2) +
              "#Z#" + NWScript.FloatToString(pos.z, 5, 2) +
              "#F#" + NWScript.FloatToString(facing, 5, 2) + "#";
    }

    public static Location StringToLocation(string s)
    {
      float facing, x, y, z;

      int idx, cnt;
      int strlen = s.Length;

      idx = NWScript.FindSubString(s, "#TAG#") + 5;
      cnt = NWScript.FindSubString(NWScript.GetSubString(s, idx, strlen - idx), "#");
      string tag = NWScript.GetSubString(s, idx, cnt);

      idx = NWScript.FindSubString(s, "#RESREF#") + 8;
      cnt = NWScript.FindSubString(NWScript.GetSubString(s, idx, strlen - idx), "#");
      string resref = NWScript.GetSubString(s, idx, cnt);

      uint area = NWScript.GetFirstArea();
      while (area != NWScript.OBJECT_INVALID)
      {
        if (NWScript.GetTag(area) == tag && NWScript.GetResRef(area) == resref)
          break;
        area = NWScript.GetNextArea();
      }

      idx = NWScript.FindSubString(s, "#X#") + 3;
      cnt = NWScript.FindSubString(NWScript.GetSubString(s, idx, strlen - idx), "#");
      x = NWScript.StringToFloat(NWScript.GetSubString(s, idx, cnt));

      idx = NWScript.FindSubString(s, "#Y#") + 3;
      cnt = NWScript.FindSubString(NWScript.GetSubString(s, idx, strlen - idx), "#");
      y = NWScript.StringToFloat(NWScript.GetSubString(s, idx, cnt));

      idx = NWScript.FindSubString(s, "#Z#") + 3;
      cnt = NWScript.FindSubString(NWScript.GetSubString(s, idx, strlen - idx), "#");
      z = NWScript.StringToFloat(NWScript.GetSubString(s, idx, cnt));

      idx = NWScript.FindSubString(s, "#F#") + 3;
      cnt = NWScript.FindSubString(NWScript.GetSubString(s, idx, strlen - idx), "#");
      facing = NWScript.StringToFloat(NWScript.GetSubString(s, idx, cnt));

      return NWScript.Location(area, NWScript.Vector(x, y, z), facing);
    }
  }
}
