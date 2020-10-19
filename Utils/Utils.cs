using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;
using System.Numerics;

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
      WebhookPlugin.SendWebHookHTTPS("discordapp.com", "/api/webhooks/737378235402289264/3-nDoj7dEw-edzjM-DDyjWFCZbs6LXACoJ9vFnOWXc8Pn2nArFEt3HiVIhHyu_lYiNUt/slack", e.Message, "AOA CRITICAL Errors");
    }
    public static void LogMessageToDMs(string message)
    {
      NWScript.SendMessageToAllDMs(message);
      WebhookPlugin.SendWebHookHTTPS("discordapp.com", "/api/webhooks/737378235402289264/3-nDoj7dEw-edzjM-DDyjWFCZbs6LXACoJ9vFnOWXc8Pn2nArFEt3HiVIhHyu_lYiNUt/slack", message, "AOA Errors");
    }

    public static void DestroyInventory(uint oContainer)
    {
      var objectsToDestroy = new List<uint> { };
      var oObj = NWScript.GetFirstItemInInventory(oContainer);

      while (NWScript.GetIsObjectValid(oObj) == 1)
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
      Vector3 pos = NWScript.GetPositionFromLocation(l);
      float facing = NWScript.GetFacingFromLocation(l);

      return "#TAG#" + NWScript.GetTag(area) + "#RESREF#" + NWScript.GetResRef(area) +
              "#X#" + NWScript.FloatToString(pos.X, 5, 2) +
              "#Y#" + NWScript.FloatToString(pos.Y, 5, 2) +
              "#Z#" + NWScript.FloatToString(pos.Z, 5, 2) +
              "#F#" + NWScript.FloatToString(facing, 5, 2) + "#";
    }

    public static double ScaleToRange(double value, double originalMin, double originalMax, double destMin, double destMax)
    {
      double result = (value - originalMin) / (originalMax - originalMin) * (destMax - destMin);
      return result + destMin;
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
      var oPartyMember = NWScript.GetFirstFactionMember(oPC, 1);

      while (NWScript.GetIsObjectValid(oPartyMember) == 1)
      {
        if (oPartyMember == oTarget)
          return true;
        oPartyMember = NWScript.GetNextFactionMember(oPC, 1);
      }
      return false;
    }
    public static void RebootTimer(uint oPC, int iTimer)
    {
      NWScript.PostString(oPC, $"REBOOT dans {iTimer} secondes !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 30.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 1, "fnt_galahad14");
      GUI_DrawWindow(oPC, 2, NWScript.SCREEN_ANCHOR_TOP_LEFT, 77, 7, 30, 5);
      iTimer -= 1;
      NWScript.DelayCommand(1.0f, () => RebootTimer(oPC, iTimer));

      if (iTimer < 6)
        PlayerPlugin.PlaySound(oPC, "gui_magbag_full", oPC);
      else
        PlayerPlugin.PlaySound(oPC, "gui_dm_alert", oPC);
    }
    public static int GUI_DrawWindow(uint oPlayer, int nStartID, int nAnchor, int nX, int nY, int nWidth, int nHeight, float fLifetime = 0.0f)
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
    public static void GUI_Draw(uint oPlayer, string sMessage, int nX, int nY, int nAnchor, int nID, float fLifeTime = 0.0f)
    {
      NWScript.PostString(oPlayer, sMessage, nX, nY, nAnchor, fLifeTime, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), nID, "fnt_es_gui");
    }
    public static void BootAllPC()
    {
      foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
      {
        if (NWScript.GetIsDM(PlayerListEntry.Key) != 1)
          NWScript.BootPC(PlayerListEntry.Key, "Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
      }
    }
    public static bool HasAnyEffect(uint oObject, params int[] effectIDs)
    {
      var eff = NWScript.GetFirstEffect(oObject);
      while (NWScript.GetIsEffectValid(eff) == 1)
      {
        if (effectIDs.Contains(NWScript.GetEffectType(eff))) 
          return true;
        eff = NWScript.GetNextEffect(oObject);
      }

      return false;
    }
    public static Boolean HasTagEffect(uint oObject, string sTag)
    {
      var eff = NWScript.GetFirstEffect(oObject);
      while (NWScript.GetIsEffectValid(eff) == 1)
      {
        if (NWScript.GetEffectTag(eff) == sTag)
          return true;
        eff = NWScript.GetNextEffect(oObject);
      }
      return false;
    }

    public static void RemoveTaggedEffect(uint oObject, string Tag)
    {
      var eff = NWScript.GetFirstEffect(oObject);
      while (NWScript.GetIsEffectValid(eff) == 1)
      { 
        if (NWScript.GetEffectTag(eff) == Tag)
        {
          NWScript.RemoveEffect(oObject, eff);
          break;
        }
        eff = NWScript.GetNextEffect(oObject);
      }
    }
    public static QuickBarSlot CreateEmptyQBS()
    {
      QuickBarSlot emptyQBS = new QuickBarSlot();
      emptyQBS.nObjectType =  0; // 0 = EMPTY

      emptyQBS.oItem = NWScript.OBJECT_INVALID;
      emptyQBS.oSecondaryItem = NWScript.OBJECT_INVALID;
      emptyQBS.nMultiClass = 0;
      emptyQBS.sResRef = "";
      emptyQBS.sCommandLabel = "";
      emptyQBS.sCommandLine = "";
      emptyQBS.sToolTip = "";
      emptyQBS.nINTParam1 = 0;
      emptyQBS.nMetaType = 0;
      emptyQBS.nDomainLevel = 0;
      emptyQBS.nAssociateType = 0;
      emptyQBS.oAssociate = NWScript.OBJECT_INVALID;

      return emptyQBS;
    }
  }
}
