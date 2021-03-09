using Discord;
using NWN.Systems;
using System;
using NWN.Core.NWNX;
using NWN.Core;
using System.Numerics;
using System.Linq;
using NWN.API;

namespace NWN
{
  public static class Utils
  {
    public static Random random = new Random();
    public static void LogMessageToDMs(string message)
    {
      switch (Config.env)
      {
        case Config.Env.Prod:
          (Bot._client.GetChannel(703964971549196339) as IMessageChannel).SendMessageAsync(message);
          break;
        case Config.Env.Bigby:
          Bot._client.GetUser(225961076448034817).SendMessageAsync(message);
          break;
        case Config.Env.Chim:
          Bot._client.GetUser(232218662080086017).SendMessageAsync(message);
          break;
      }
    }
    public static void DestroyInventory(uint oContainer)
    {
      foreach (NwItem item in oContainer.ToNwObject<NwGameObject>().Items)
        item.Destroy();
    }
    public static void DestroyEquippedItems(uint oCreature)
    {
      for (int i = 0; i < 17; i++)
        NWScript.DestroyObject(NWScript.GetItemInSlot(i, oCreature));
    }

    public static double ScaleToRange(double value, double originalMin, double originalMax, double destMin, double destMax)
    {
      double result = (value - originalMin) / (originalMax - originalMin) * (destMax - destMin);
      return result + destMin;
    }

    public static API.Location GetLocationFromDatabase(string areaTag, Vector3 position, float facing)
    {
      return API.Location.Create(NwModule.Instance.Areas.Where(a => a.Tag == areaTag).FirstOrDefault(), position, facing);
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
      foreach (NwPlayer oPC in NwModule.Instance.Players)
        oPC.BootPlayer("Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
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
      emptyQBS.nObjectType = 0; // 0 = EMPTY

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
    public static TimeSpan StripTimeSpanMilliseconds(TimeSpan timespan)
    {
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
    }
    public static int TranslateEngineAnimation(int nAnimation)
    {
      switch (nAnimation)
      {
        case 0: nAnimation = NWScript.ANIMATION_LOOPING_PAUSE; break;
        case 52: nAnimation = NWScript.ANIMATION_LOOPING_PAUSE2; break;
        case 30: nAnimation = NWScript.ANIMATION_LOOPING_LISTEN; break;
        case 32: nAnimation = NWScript.ANIMATION_LOOPING_MEDITATE; break;
        case 33: nAnimation = NWScript.ANIMATION_LOOPING_WORSHIP; break;
        case 48: nAnimation = NWScript.ANIMATION_LOOPING_LOOK_FAR; break;
        case 36: nAnimation = NWScript.ANIMATION_LOOPING_SIT_CHAIR; break;
        case 47: nAnimation = NWScript.ANIMATION_LOOPING_SIT_CROSS; break;
        case 38: nAnimation = NWScript.ANIMATION_LOOPING_TALK_NORMAL; break;
        case 39: nAnimation = NWScript.ANIMATION_LOOPING_TALK_PLEADING; break;
        case 40: nAnimation = NWScript.ANIMATION_LOOPING_TALK_FORCEFUL; break;
        case 41: nAnimation = NWScript.ANIMATION_LOOPING_TALK_LAUGHING; break;
        case 59: nAnimation = NWScript.ANIMATION_LOOPING_GET_LOW; break;
        case 60: nAnimation = NWScript.ANIMATION_LOOPING_GET_MID; break;
        case 57: nAnimation = NWScript.ANIMATION_LOOPING_PAUSE_TIRED; break;
        case 58: nAnimation = NWScript.ANIMATION_LOOPING_PAUSE_DRUNK; break;
        case 6: nAnimation = NWScript.ANIMATION_LOOPING_DEAD_FRONT; break;
        case 8: nAnimation = NWScript.ANIMATION_LOOPING_DEAD_BACK; break;
        case 15: nAnimation = NWScript.ANIMATION_LOOPING_CONJURE1; break;
        case 16: nAnimation = NWScript.ANIMATION_LOOPING_CONJURE2; break;
        case 93: nAnimation = NWScript.ANIMATION_LOOPING_SPASM; break;
        case 97: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM1; break;
        case 98: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM2; break;
        case 101: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM3; break;
        case 102: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM4; break;
        case 103: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM5; break;
        case 104: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM6; break;
        case 105: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM7; break;
        case 106: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM8; break;
        case 107: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM9; break;
        case 108: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM10; break;
        case 109: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM11; break;
        case 110: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM12; break;
        case 111: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM13; break;
        case 112: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM14; break;
        case 113: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM15; break;
        case 114: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM16; break;
        case 115: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM17; break;
        case 116: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM18; break;
        case 117: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM19; break;
        case 118: nAnimation = NWScript.ANIMATION_LOOPING_CUSTOM20; break;
        case 119: nAnimation = NWScript.ANIMATION_MOUNT1; break;
        case 120: nAnimation = NWScript.ANIMATION_DISMOUNT1; break;
        case 53: nAnimation = NWScript.ANIMATION_FIREFORGET_HEAD_TURN_LEFT; break;
        case 54: nAnimation = NWScript.ANIMATION_FIREFORGET_HEAD_TURN_RIGHT; break;
        case 55: nAnimation = NWScript.ANIMATION_FIREFORGET_PAUSE_SCRATCH_HEAD; break;
        case 56: nAnimation = NWScript.ANIMATION_FIREFORGET_PAUSE_BORED; break;
        case 34: nAnimation = NWScript.ANIMATION_FIREFORGET_SALUTE; break;
        case 35: nAnimation = NWScript.ANIMATION_FIREFORGET_BOW; break;
        case 37: nAnimation = NWScript.ANIMATION_FIREFORGET_STEAL; break;
        case 29: nAnimation = NWScript.ANIMATION_FIREFORGET_GREETING; break;
        case 28: nAnimation = NWScript.ANIMATION_FIREFORGET_TAUNT; break;
        case 44: nAnimation = NWScript.ANIMATION_FIREFORGET_VICTORY1; break;
        case 45: nAnimation = NWScript.ANIMATION_FIREFORGET_VICTORY2; break;
        case 46: nAnimation = NWScript.ANIMATION_FIREFORGET_VICTORY3; break;
        case 71: nAnimation = NWScript.ANIMATION_FIREFORGET_READ; break;
        case 70: nAnimation = NWScript.ANIMATION_FIREFORGET_DRINK; break;
        case 90: nAnimation = NWScript.ANIMATION_FIREFORGET_DODGE_SIDE; break;
        case 91: nAnimation = NWScript.ANIMATION_FIREFORGET_DODGE_DUCK; break;
        case 23: nAnimation = NWScript.ANIMATION_FIREFORGET_SPASM; break;
        default: nAnimation = -1; break;
      }

      return nAnimation;
    }
  }
}
