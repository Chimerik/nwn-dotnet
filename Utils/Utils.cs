using Discord;
using NWN.Systems;
using System;
using System.Numerics;
using System.Linq;
using Anvil.API;
using System.Collections.Generic;
using NLog;
using NWN.Core;

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
      foreach (Effect eff in oTarget.ActiveEffects.Where(e => e.Tag == Tag))
        oTarget.RemoveEffect(eff);
    }
    public static TimeSpan StripTimeSpanMilliseconds(TimeSpan timespan)
    {
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
    }
    public static string FormatTimeSpan(TimeSpan timespan)
    {
      string formattedTimespan = "";

      if (timespan.TotalSeconds < 1)
        return "Immédiat";

      if (timespan.TotalDays >= 1)
        formattedTimespan = $"{timespan.TotalDays}j ";

      if (timespan.TotalHours >= 1)
        formattedTimespan += $"{timespan.Hours}h ";

      if (timespan.TotalMinutes >= 1)
        formattedTimespan += $"{timespan.Minutes}m ";

      if (timespan.TotalHours < 1)
        formattedTimespan += $"{timespan.Seconds}s ";

      return formattedTimespan;
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
        case 93: return Animation.LoopingCustom1;
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

      await SqLiteUtils.InsertQueryAsync("messenger",
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

      var result = await SqLiteUtils.SelectQueryAsync("PlayerAccounts",
          new List<string>() { { "discordId" } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

      if (result != null && result.Count > 0)
      {
        await Bot._client.GetUser(ulong.Parse(result[0][0])).SendMessageAsync(message);
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
      creature.VisualTransform.Rotation = new Vector3(0, 0, 0);
      creature.VisualTransform.Translation = new Vector3(0, 0, 0);

      if (creature.IsPlayerControlled)
        creature.ControllingPlayer.CameraHeight = 0;
    }
    public static IntPtr GffGetFieldType(IntPtr jGff, string sLabel)
    {
      return NWScript.JsonPointer(jGff, "/" + sLabel + "/type");
    }
    public static IntPtr GffGetField(IntPtr jGff, string sLabel, string sType)
    {
      IntPtr jType = GffGetFieldType(jGff, sLabel);
      if (jType == NWScript.JsonNull())
        return jType;
      else if (jType != NWScript.JsonString(sType))
        return NWScript.JsonNull("field type does not match");
      else
        return GffGetFieldValue(jGff, sLabel);
    }
    public static IntPtr GffGetFieldValue(IntPtr jGff, string sLabel)
    {
      return NWScript.JsonPointer(jGff, "/" + sLabel + "/value");
    }
    public static IntPtr GffGetByte(IntPtr jGff, string sLabel)
    {
      return GffGetField(jGff, sLabel, "byte");
    }
    public static string Util_GetIconResref(NwItem oItem)
    {
      switch (oItem.BaseItemType)
      {
        case BaseItemType.Cloak: // Cloaks use PLTs so their default icon doesn't really work
          return "iit_cloak";
        case BaseItemType.SpellScroll: // Scrolls get their icon from the cast spell property
        case BaseItemType.EnchantedScroll:

          if (oItem.HasItemProperty(ItemPropertyType.CastSpell))
            return ItemPropertySpells2da.spellsTable.GetSpellDataEntry(oItem.ItemProperties.FirstOrDefault(ip => ip.PropertyType == ItemPropertyType.CastSpell).SubType).icon;

          break;
        default:

          if (BaseItems2da.baseItemTable.GetBaseItemDataEntry(oItem.BaseItemType).modelType == 0) // Create the icon resref for simple modeltype items
          {
            IntPtr jSimpleModel = GffGetByte(NWScript.ObjectToJson(oItem), "ModelPart1");
            if (NWScript.JsonGetType(jSimpleModel) == NWScript.JSON_TYPE_INTEGER)
            {
              string sSimpleModelId = NWScript.JsonGetInt(jSimpleModel).ToString();
              while (sSimpleModelId.Length < 3)// Padding...
              {
                sSimpleModelId = "0" + sSimpleModelId;
              }

              string sDefaultIcon = BaseItems2da.baseItemTable.GetBaseItemDataEntry(oItem.BaseItemType).defaultIcon;
              switch (oItem.BaseItemType)
              {
                case BaseItemType.MiscSmall:
                case BaseItemType.CraftMaterialSmall:
                  sDefaultIcon = "iit_smlmisc_" + sSimpleModelId;
                  break;
                case BaseItemType.MiscMedium:
                case BaseItemType.CraftMaterialMedium:
                case (BaseItemType)112:/* Crafting Base Material */
                  sDefaultIcon = "iit_midmisc_" + sSimpleModelId;
                  break;
                case BaseItemType.MiscLarge:
                  sDefaultIcon = "iit_talmisc_" + sSimpleModelId;
                  break;
                case BaseItemType.MiscThin:
                  sDefaultIcon = "iit_thnmisc_" + sSimpleModelId;
                  break;
              }

              int nLength = sDefaultIcon.Length;
              if (sDefaultIcon.Substring(nLength - 4, 1) == "_")// Some items have a default icon of xx_yyy_001, we strip the last 4 symbols if that is the case
                sDefaultIcon = sDefaultIcon.Remove(nLength - 4);
              string sIcon = sDefaultIcon + "_" + sSimpleModelId;
              if (NWScript.ResManGetAliasFor(sIcon, NWScript.RESTYPE_TGA) != "")// Check if the icon actually exists, if not, we'll fall through and return the default icon
                return sIcon;
            }
          }

          break;
      }

      // For everything else use the item's default icon
      return BaseItems2da.baseItemTable.GetBaseItemDataEntry(oItem.BaseItemType).defaultIcon;
    }
    public static IntPtr Util_GetModelPart(string sDefaultIcon, string sType, IntPtr jPart)
    {
      if (NWScript.JsonGetType(jPart) == NWScript.JSON_TYPE_INTEGER)
      {
        string sModelPart = NWScript.JsonGetInt(jPart).ToString();
        while (sModelPart.Length < 3)
        {
          sModelPart = "0" + sModelPart;
        }

        string sIcon = sDefaultIcon + sType + sModelPart;
        if (NWScript.ResManGetAliasFor(sIcon, NWScript.RESTYPE_TGA) != "")
          return NWScript.JsonString(sIcon);
      }

      return NWScript.JsonString("");
    }
    public static IntPtr Util_GetComplexIconData(IntPtr jItem, BaseItemType nBaseItem)
    {
      BaseItemTable.Entry entry = BaseItems2da.baseItemTable.GetBaseItemDataEntry(nBaseItem);
      if (entry.modelType == 2)
      {
        string sDefaultIcon = entry.defaultIcon;
        IntPtr jComplexIcon = NWScript.JsonObject();
        jComplexIcon = NWScript.JsonObjectSet(jComplexIcon, "top", Util_GetModelPart(sDefaultIcon, "_t_", GffGetByte(jItem, "ModelPart3")));
        jComplexIcon = NWScript.JsonObjectSet(jComplexIcon, "middle", Util_GetModelPart(sDefaultIcon, "_m_", GffGetByte(jItem, "ModelPart2")));
        jComplexIcon = NWScript.JsonObjectSet(jComplexIcon, "bottom", Util_GetModelPart(sDefaultIcon, "_b_", GffGetByte(jItem, "ModelPart1")));

        return jComplexIcon;
      }

      return NWScript.JsonNull();
    }
  }
}
