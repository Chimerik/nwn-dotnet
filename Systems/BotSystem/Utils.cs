using Discord.Commands;
using NWN.Core;

namespace NWN.Systems
{
  class DiscordUtils
  {
    public static int CheckPlayerCredentialsFromDiscord(SocketCommandContext context, string sPCName)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT pc.ROWID from PlayerAccounts " +
        $"LEFT join playerCharacters pc on pc.accountId = PlayerAccounts.ROWID WHERE discordId = @discordId and pc.characterName like '{sPCName}%'");
      NWScript.SqlBindString(query, "@discordId", context.User.Id.ToString());
      //NWScript.SqlBindString(query, "@characterName", sPCName);
      if(NWScript.SqlStep(query) == 1)
        return NWScript.SqlGetInt(query, 0);

      return 0;
    }
    public static string GetPlayerStaffRankFromDiscord(ulong UserId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT rank from PlayerAccounts WHERE discordId = @discordId");
      NWScript.SqlBindString(query, "@discordId", UserId.ToString());
      if (NWScript.SqlStep(query) == 1)
        return NWScript.SqlGetString(query, 0);

      return "";
    }
    public static int GetPlayerAccountIdFromDiscord(ulong UserId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT ROWID from PlayerAccounts WHERE discordId = @discordId");
      NWScript.SqlBindString(query, "@discordId", UserId.ToString());
      if (NWScript.SqlStep(query) == 1)
        return NWScript.SqlGetInt(query, 0);

      return -1;
    }
  }
}
