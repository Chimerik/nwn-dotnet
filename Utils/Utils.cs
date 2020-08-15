using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.Systems;

namespace NWN
{
  public static class Utils
  {
    public static Random random = new Random();

    public static void LogException(Exception e)
    {
      Console.WriteLine(e.Message);
      NWScript.SendMessageToAllDMs(e.Message);
      NWScript.WriteTimestampedLogEntry(e.Message);
      // TODO : Une fois dotnet.core mis à jour, envoyer le message sur Discord via nwnx.webhook
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
    public static Boolean IsPartyMember(uint oPC, uint oTarget)
    {
      // Get the first PC party member
      var oPartyMember = NWScript.GetFirstFactionMember(oPC, true);

      while (NWScript.GetIsObjectValid(oPartyMember))
      {
        if (oPartyMember == oTarget)
          return true;
        oPartyMember = NWScript.GetNextFactionMember(oPC, true);
      }
      return false;
    }
    public static void RebootTimer(uint oPC, int iTimer)
    {
      NWScript.PostString(oPC, $"REBOOT dans {iTimer} secondes !", 80, 10, ScreenAnchor.TopLeft, 30.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 1, "fnt_galahad14");
      GUI_DrawWindow(oPC, 2, ScreenAnchor.TopLeft, 77, 7, 30, 5);
      iTimer -= 1;
      NWScript.DelayCommand(1.0f, () => RebootTimer(oPC, iTimer));

      if (iTimer < 6)
        NWNX.Player.PlaySound(oPC, "gui_magbag_full", oPC);
      else
        NWNX.Player.PlaySound(oPC, "gui_dm_alert", oPC);
    }
    public static int GUI_DrawWindow(uint oPlayer, int nStartID, ScreenAnchor nAnchor, int nX, int nY, int nWidth, int nHeight, float fLifetime = 0.0f)
    {
      string sTop = "a";
      string sMiddle = "d";
      string sBottom = "h";

      int i;
      for (i = 0; i < nWidth; i++)
      {
        sTop += "b";
        sMiddle += "i";
        sBottom += "e";
      }

      sTop += "c";
      sMiddle += "f";
      sBottom += "g";

      GUI_Draw(oPlayer, sTop, nX, nY, nAnchor, nStartID++, fLifetime);
      for (i = 0; i < nHeight; i++)
      {
        GUI_Draw(oPlayer, sMiddle, nX, ++nY, nAnchor, nStartID++, fLifetime);
      }
      GUI_Draw(oPlayer, sBottom, nX, ++nY, nAnchor, nStartID, fLifetime);

      return nHeight + 2;
    }
    public static void GUI_Draw(uint oPlayer, string sMessage, int nX, int nY, ScreenAnchor nAnchor, int nID, float fLifeTime = 0.0f)
    {
      NWScript.PostString(oPlayer, sMessage, nX, nY, nAnchor, fLifeTime, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), nID, "fnt_es_gui");
    }
    public static void BootAllPC()
    {
      foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
      {
        if (!NWScript.GetIsDM(PlayerListEntry.Key))
          NWScript.BootPC(PlayerListEntry.Key, "Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
      }
    }
  }
}
