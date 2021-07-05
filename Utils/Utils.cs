using Discord;
using NWN.Systems;
using System;
using NWN.Core.NWNX;
using NWN.Core;
using System.Numerics;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using System.Collections.Generic;
using NLog;

namespace NWN
{
  public static class Utils
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static Random random = new Random();
    public static void LogMessageToDMs(string message)
    {
      Log.Info(message);

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
    public static void DestroyInventory(NwCreature oContainer)
    {
      foreach (NwItem item in oContainer.Inventory.Items)
        item.Destroy();
    }
    public static void DestroyInventory(NwPlaceable oContainer)
    {
      foreach (NwItem item in oContainer.Inventory.Items)
        item.Destroy();
    }
    public static void DestroyInventory(NwStore oContainer)
    {
      foreach (NwItem item in oContainer.Items)
        item.Destroy();
    }
    public static void DestroyEquippedItems(NwCreature oCreature)
    {
      for (int i = 0; i < 17; i++)
        oCreature.GetItemInSlot((InventorySlot)i)?.Destroy();
    }

    public static double ScaleToRange(double value, double originalMin, double originalMax, double destMin, double destMax)
    {
      double result = (value - originalMin) / (originalMax - originalMin) * (destMax - destMin);
      return result + destMin;
    }

    public static Location GetLocationFromDatabase(string areaTag, string position, float facing)
    {
      Vector3 pos;

      if (position.Contains(","))
      {
        position = position.Replace("<", "");
        position = position.Replace(">", "");
        string[] splitString = position.Split(",");
        pos = new Vector3(float.TryParse(splitString[0], out float X) ? X : 0, float.TryParse(splitString[1], out float Y) ? Y : 0, float.TryParse(splitString[2], out float Z) ? Z : 0);
      }
      else
      {
        string[] splitString = position.Split(":");
        pos = new Vector3(float.TryParse(splitString[0], out float X) ? X : 0, float.TryParse(splitString[1], out float Y) ? Y : 0, float.TryParse(splitString[2], out float Z) ? Z : 0);
      }

      return Location.Create(NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == areaTag), pos, facing);
    }
    public static void BootAllPC()
    {
      foreach (NwPlayer oPC in NwModule.Instance.Players)
        oPC.BootPlayer("Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
    }
    public static void RemoveTaggedEffect(NwGameObject oTarget, string Tag)
    {
      if (oTarget is NwCreature oCreature)
      {
        foreach (Effect eff in oCreature.ActiveEffects.Where(e => e.Tag == Tag))
          oTarget.RemoveEffect(eff);
      }
      else if (oTarget is NwPlaceable oPlaceable)
      {
        foreach (Effect eff in oPlaceable.ActiveEffects.Where(e => e.Tag == Tag))
          oTarget.RemoveEffect(eff);
      }
    }
    public static QuickBarSlot CreateEmptyQBS()
    {
      QuickBarSlot emptyQBS = new QuickBarSlot();
      emptyQBS.nObjectType = 0; // 0 = EMPTY
      emptyQBS.oItem = (uint)ObjectTypes.Invalid;
      emptyQBS.oSecondaryItem = (uint)ObjectTypes.Invalid;
      emptyQBS.nMultiClass = 0;
      emptyQBS.sResRef = "";
      emptyQBS.sCommandLabel = "";
      emptyQBS.sCommandLine = "";
      emptyQBS.sToolTip = "";
      emptyQBS.nINTParam1 = 0;
      emptyQBS.nMetaType = 0;
      emptyQBS.nDomainLevel = 0;
      emptyQBS.nAssociateType = 0;
      emptyQBS.oAssociate = (uint)ObjectTypes.Invalid;

      return emptyQBS;
    }
    public static TimeSpan StripTimeSpanMilliseconds(TimeSpan timespan)
    {
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
    }
    public static Animation TranslateEngineAnimation(int nAnimation)
    {
      Animation translatedAnim;

      switch (nAnimation)
      {
        case 0: translatedAnim = Animation.LoopingPause; break;
        case 52: translatedAnim = Animation.LoopingPause2; break;
        case 30: translatedAnim = Animation.LoopingListen; break;
        case 32: translatedAnim = Animation.LoopingMeditate; break;
        case 33: translatedAnim = Animation.LoopingWorship; break;
        case 48: translatedAnim = Animation.LoopingLookFar; break;
        case 36: translatedAnim = Animation.LoopingSitChair; break;
        case 47: translatedAnim = Animation.LoopingSitCross; break;
        case 38: translatedAnim = Animation.LoopingTalkNormal; break;
        case 39: translatedAnim = Animation.LoopingTalkPleading; break;
        case 40: translatedAnim = Animation.LoopingTalkForceful; break;
        case 41: translatedAnim = Animation.LoopingTalkLaughing; break;
        case 59: translatedAnim = Animation.LoopingGetLow; break;
        case 60: translatedAnim = Animation.LoopingGetMid; break;
        case 57: translatedAnim = Animation.LoopingPauseTired; break;
        case 58: translatedAnim = Animation.LoopingPauseDrunk; break;
        case 6: translatedAnim = Animation.LoopingDeadFront; break;
        case 8: translatedAnim = Animation.LoopingDeadBack; break;
        case 15: translatedAnim = Animation.LoopingConjure1; break;
        case 16: translatedAnim = Animation.LoopingConjure2; break;
        case 93: translatedAnim = Animation.LoopingCustom1 ; break;
        case 98: translatedAnim = Animation.LoopingCustom2; break;
        case 101: translatedAnim = Animation.LoopingCustom3; break;
        case 102: translatedAnim = Animation.LoopingCustom4; break;
        case 103: translatedAnim = Animation.LoopingCustom5; break;
        case 104: translatedAnim = Animation.LoopingCustom6; break;
        case 105: translatedAnim = Animation.LoopingCustom7; break;
        case 106: translatedAnim = Animation.LoopingCustom8; break;
        case 107: translatedAnim = Animation.LoopingCustom9; break;
        case 108: translatedAnim = Animation.LoopingCustom10; break;
        case 109: translatedAnim = Animation.LoopingCustom11; break;
        case 110: translatedAnim = Animation.LoopingCustom12; break;
        case 111: translatedAnim = Animation.LoopingCustom13; break;
        case 112: translatedAnim = Animation.LoopingCustom14; break;
        case 113: translatedAnim = Animation.LoopingCustom15; break;
        case 114: translatedAnim = Animation.LoopingCustom16; break;
        case 115: translatedAnim = Animation.LoopingCustom17; break;
        case 116: translatedAnim = Animation.LoopingCustom18; break;
        case 117: translatedAnim = Animation.LoopingCustom19; break;
        case 118: translatedAnim = Animation.LoopingCustom20; break;
        case 119: translatedAnim = Animation.Mount1; break;
        case 120: translatedAnim = Animation.Dismount1; break;
        case 53: translatedAnim = Animation.FireForgetHeadTurnLeft; break;
        case 54: translatedAnim = Animation.FireForgetHeadTurnRight; break;
        case 55: translatedAnim = Animation.FireForgetPauseScratchHead; break;
        case 56: translatedAnim = Animation.FireForgetPauseBored; break;
        case 34: translatedAnim = Animation.FireForgetSalute; break;
        case 35: translatedAnim = Animation.FireForgetBow; break;
        case 37: translatedAnim = Animation.FireForgetSteal; break;
        case 29: translatedAnim = Animation.FireForgetGreeting; break;
        case 28: translatedAnim = Animation.FireForgetTaunt; break;
        case 44: translatedAnim = Animation.FireForgetVictory1; break;
        case 45: translatedAnim = Animation.FireForgetVictory2; break;
        case 46: translatedAnim = Animation.FireForgetVictory3; break;
        case 71: translatedAnim = Animation.FireForgetRead; break;
        case 70: translatedAnim = Animation.FireForgetDrink; break;
        case 90: translatedAnim = Animation.FireForgetDodgeSide; break;
        case 91: translatedAnim = Animation.FireForgetDodgeDuck; break;
        case 23: translatedAnim = Animation.LoopingSpasm; break;
        default: translatedAnim = Animation.FireForgetPauseBored; break;
      }

      return (Animation)nAnimation;
    }
    public static async void SendMailToPC(int characterId, string senderName, string title, string message)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      SqLiteUtils.InsertQuery("messenger",
          new List<string[]>() {
            new string[] { "characterId", characterId.ToString() },
            new string[] { "senderName", senderName },
            new string[] { "title", title },
          new string[] { "message", message },
          new string[] { "sentDate", DateTime.Now.ToLongDateString() },
          new string[] { "read", "0" } });
    }
    public static async void SendDiscordPMToPlayer(int characterId, string message)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      var result = SqLiteUtils.SelectQuery("PlayerAccounts",
          new List<string>() { { "discordId" } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

      if (result.Result != null)
      {
        await Bot._client.GetUser(ulong.Parse(result.Result.GetString(0))).SendMessageAsync(message);
      }
    }
    public static async void SendItemToPCStorage(int characterId, NwItem item)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      var result = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "storage" } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

      if (result.Result != null)
      {
        NwStore storage = SqLiteUtils.StoreSerializationFormatProtection(result.Result, 0, NwModule.Instance.StartingLocation);
        item.Clone(storage);
        item.Destroy();

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "storage", storage.Serialize().ToBase64EncodedString() } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

        storage.Destroy();
      }
      else
      {
        LogMessageToDMs($"Impossible de trouver le storage du pj {characterId} et d'y déposer un objet !");
      }
    }
    public static void ResetVisualTransform(NwCreature creature)
    {
      creature.VisualTransform.Rotation = new Vector3(0, 0 ,0);
      creature.VisualTransform.Translation = new Vector3(0, 0, 0);

      if (creature.IsPlayerControlled)
        creature.ControllingPlayer.CameraHeight = 0;
    }
  }
}
