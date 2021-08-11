using Discord;
using System;
using System.Numerics;
using System.Linq;
using Anvil.API;
using System.Collections.Generic;
using NLog;

namespace Utils
{
  public static class MiscUtils
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static Random random = new Random();

    public static void LogMessageToDMs(string message)
    {
      Log.Info(message);

      switch (Config.env)
      {
        case Config.Env.Prod:
          (DiscordUtils._client.GetChannel(703964971549196339) as IMessageChannel).SendMessageAsync(message);
          break;
        case Config.Env.Bigby:
          DiscordUtils._client.GetUser(225961076448034817).SendMessageAsync(message);
          break;
        case Config.Env.Chim:
          DiscordUtils._client.GetUser(232218662080086017).SendMessageAsync(message);
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
      foreach (Effect eff in oTarget.ActiveEffects.Where(e => e.Tag == Tag))
        oTarget.RemoveEffect(eff);
    }
    public static TimeSpan StripTimeSpanMilliseconds(TimeSpan timespan)
    {
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
    }
    public static Animation TranslateEngineAnimation(int nAnimation)
    {
      switch (nAnimation)
      {
        case 0: return Animation.LoopingPause;
        case 52: return Animation.LoopingPause2;
        case 30: return Animation.LoopingListen;
        case 32: return Animation.LoopingMeditate; 
        case 33: return Animation.LoopingWorship; 
        case 48: return Animation.LoopingLookFar; 
        case 36: return Animation.LoopingSitChair; 
        case 47: return Animation.LoopingSitCross; 
        case 38: return Animation.LoopingTalkNormal; 
        case 39: return Animation.LoopingTalkPleading;
        case 40: return Animation.LoopingTalkForceful;
        case 41: return Animation.LoopingTalkLaughing;
        case 59: return Animation.LoopingGetLow;
        case 60: return Animation.LoopingGetMid;
        case 57: return Animation.LoopingPauseTired;
        case 58: return Animation.LoopingPauseDrunk;
        case 6: return Animation.LoopingDeadFront;
        case 8: return Animation.LoopingDeadBack;
        case 15: return Animation.LoopingConjure1;
        case 16: return Animation.LoopingConjure2;
        case 93: return Animation.LoopingCustom1 ;
        case 98: return Animation.LoopingCustom2;
        case 101: return Animation.LoopingCustom3;
        case 102: return Animation.LoopingCustom4;
        case 103: return Animation.LoopingCustom5;
        case 104: return Animation.LoopingCustom6;
        case 105: return Animation.LoopingCustom7;
        case 106: return Animation.LoopingCustom8;
        case 107: return Animation.LoopingCustom9;
        case 108: return Animation.LoopingCustom10;
        case 109: return Animation.LoopingCustom11;
        case 110: return Animation.LoopingCustom12;
        case 111: return Animation.LoopingCustom13;
        case 112: return Animation.LoopingCustom14;
        case 113: return Animation.LoopingCustom15;
        case 114: return Animation.LoopingCustom16;
        case 115: return Animation.LoopingCustom17;
        case 116: return Animation.LoopingCustom18;
        case 117: return Animation.LoopingCustom19;
        case 118: return Animation.LoopingCustom20;
        case 119: return Animation.Mount1;
        case 120: return Animation.Dismount1;
        case 53: return Animation.FireForgetHeadTurnLeft;
        case 54: return Animation.FireForgetHeadTurnRight;
        case 55: return Animation.FireForgetPauseScratchHead;
        case 56: return Animation.FireForgetPauseBored;
        case 34: return Animation.FireForgetSalute;
        case 35: return Animation.FireForgetBow;
        case 37: return Animation.FireForgetSteal;
        case 29: return Animation.FireForgetGreeting;
        case 28: return Animation.FireForgetTaunt;
        case 44: return Animation.FireForgetVictory1;
        case 45: return Animation.FireForgetVictory2;
        case 46: return Animation.FireForgetVictory3;
        case 71: return Animation.FireForgetRead;
        case 70: return Animation.FireForgetDrink;
        case 90: return Animation.FireForgetDodgeSide;
        case 91: return Animation.FireForgetDodgeDuck;
        case 23: return Animation.LoopingSpasm;
        default: return Animation.FireForgetPauseBored;
      }
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
        await DiscordUtils._client.GetUser(ulong.Parse(result.Result.GetString(0))).SendMessageAsync(message);
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
