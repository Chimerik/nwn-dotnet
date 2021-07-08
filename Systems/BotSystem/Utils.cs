using System.Collections.Generic;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  class DiscordUtils
  {
    public static int CheckPlayerCredentialsFromDiscord(SocketCommandContext context, string sPCName)
    {
      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT pc.ROWID from PlayerAccounts " +
        $"LEFT join playerCharacters pc on pc.accountId = PlayerAccounts.ROWID WHERE discordId = @discordId and pc.characterName like '{sPCName}%'");
      query.BindParam("@discordId", context.User.Id.ToString());

      query.Execute();
      if (query.Result != null)
        return query.Result.GetInt(0);

      return 0;
    }
    public static string GetPlayerStaffRankFromDiscord(ulong UserId)
    {
      var result = SqLiteUtils.SelectQuery("PlayerAccounts",
        new List<string>() { { "rank" } },
        new List<string[]>() { new string[] { "discordId", UserId.ToString() } });

      if (result.Result != null)
        return result.Result.GetString(0);

      return "";
    }
    public static int GetPlayerAccountIdFromDiscord(ulong UserId)
    {
      var result = SqLiteUtils.SelectQuery("PlayerAccounts",
        new List<string>() { { "ROWID" } },
        new List<string[]>() { new string[] { "discordId", UserId.ToString() } });

      if (result.Result != null)
        return result.Result.GetInt(0);

      return -1;
    }
  }
}
