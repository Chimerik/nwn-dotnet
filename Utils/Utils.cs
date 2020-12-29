﻿using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems;
using System.Numerics;
using Discord;
using Discord.Commands;
using Microsoft.Data.Sqlite;

namespace NWN
{
  public static class Utils
  {
    public static Random random = new Random();

    public static void LogException(Exception e)
    {
      Console.WriteLine(e.Message);
      NWScript.WriteTimestampedLogEntry(e.Message);

      switch (Config.env)
      {
        case Config.Env.Prod:
          (Bot._client.GetChannel(703964971549196339) as IMessageChannel).SendMessageAsync(Module.currentScript + " : " + e.Message);
          break;
        case Config.Env.Bigby:
          Bot._client.GetUser(225961076448034817).SendMessageAsync(Module.currentScript + " : " + e.Message);
          break;
        case Config.Env.Chim:
          Bot._client.GetUser(232218662080086017).SendMessageAsync(Module.currentScript + " : " + e.Message);
          break;
      }
    }
    public static void LogMessageToDMs(string message)
    {
      switch(Config.env)
      {
        case Config.Env.Prod:
          (Bot._client.GetChannel(703964971549196339) as IMessageChannel).SendMessageAsync(Module.currentScript + " : " + message);
          break;
        case Config.Env.Bigby:
          Bot._client.GetUser(225961076448034817).SendMessageAsync(Module.currentScript + " : " + message);
          break;
        case Config.Env.Chim:
          Bot._client.GetUser(232218662080086017).SendMessageAsync(Module.currentScript + " : " + message);
          break;
      }
    }

    public static void DestroyInventory(uint oContainer)
    {
      var oObj = NWScript.GetFirstItemInInventory(oContainer);

      while (NWScript.GetIsObjectValid(oObj) == 1)
      {
        NWScript.DestroyObject(oObj);
        oObj = NWScript.GetNextItemInInventory(oContainer);
      }
    }
    public static void DestroyEquippedItems(uint oCreature)
    {
      for(int i = 0; i < 17; i++)
        NWScript.DestroyObject(NWScript.GetItemInSlot(i, oCreature));
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

    public static Location GetLocationFromDatabase(string areaTag, Vector3 position, float facing)
    {
      uint area = NWScript.GetFirstArea();
      while (Convert.ToBoolean(NWScript.GetIsObjectValid(area)))
      {
        if (NWScript.GetTag(area) == areaTag)
          break;
        area = NWScript.GetNextArea();
      }

      return NWScript.Location(area, position, facing);
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
      foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
      {
        if (NWScript.GetIsDM(PlayerListEntry.Key) != 1)
          NWScript.BootPC(PlayerListEntry.Key, "Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");

        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO moduleInfo (year, month, day, hour, minute, second) VALUES (@year, @month, @day, @hour, @minute, @second)");
        NWScript.SqlBindInt(query, "@year", NWScript.GetCalendarYear());
        NWScript.SqlBindInt(query, "@month", NWScript.GetCalendarMonth());
        NWScript.SqlBindInt(query, "@day", NWScript.GetCalendarDay());
        NWScript.SqlBindInt(query, "@hour", NWScript.GetTimeHour());
        NWScript.SqlBindInt(query, "@minute", NWScript.GetTimeMinute());
        NWScript.SqlBindInt(query, "@second", NWScript.GetTimeSecond());
        NWScript.SqlStep(query);

        AdminPlugin.ShutdownServer();
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
    public static TimeSpan StripTimeSpanMilliseconds(TimeSpan timespan)
    {
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
    }
    public static int CheckPlayerCredentialsFromDiscord(SocketCommandContext context, string sPCName)
    {
      using (var connection = new SqliteConnection($"{ModuleSystem.db_path}"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        SELECT pc.ROWID
        FROM PlayerAccounts
        LEFT join playerCharacters pc on pc.accountId = PlayerAccounts.ROWID
        WHERE discordId = $discordId and pc.characterName = $characterName
        ";
        command.Parameters.AddWithValue("$discordId", context.User.Id);
        command.Parameters.AddWithValue("$characterName", sPCName);

        int result = 0;

        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            result = reader.GetInt32(0);
          }
        }

        if (result > 0)
          return result;
      }

      return 0;
    }
  }
}
