using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSetRoleplayBonusCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.IsDM)
      {
        if (ctx.oTarget != null)
        {
          if (((string)options.positional[0]).Length == 0)
          {
            ctx.oSender.SendServerMessage($"Le bonus roleplay de {ctx.oTarget.Name.ColorString(Color.WHITE)} est de {ObjectPlugin.GetInt(ctx.oTarget, "_BRP").ToString().ColorString(Color.WHITE)}", Color.PINK);
          }
          else
          {
            if (int.TryParse((string)options.positional[0], out int iBRP))
            {
              if (iBRP > -1 && iBRP < 5)
              {
                if (PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player))
                {
                  player.bonusRolePlay = iBRP;
                  ctx.oSender.SendServerMessage($"Le bonus roleplay de {ctx.oTarget.Name.ColorString(Color.WHITE)} est de {player.bonusRolePlay.ToString().ColorString(Color.WHITE)}", Color.PINK);
                  ctx.oTarget.SendServerMessage($"Votre bonus roleplay est désormais de {player.bonusRolePlay.ToString().ColorString(Color.WHITE)}", Color.TEAL);

                  var updateQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE PlayerAccounts SET bonusRolePlay = @bonusRolePlay where rowid = @rowid");
                  NWScript.SqlBindInt(updateQuery, "@bonusRolePlay", player.bonusRolePlay);
                  NWScript.SqlBindInt(updateQuery, "@rowid", player.accountId);
                  NWScript.SqlStep(updateQuery);
                }
              }
              else
                ctx.oSender.SendServerMessage("Le bonus roleplay doit être compris entre 0 et 4", Color.ORANGE);
            }
            else
              if (PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player))
                ctx.oSender.SendServerMessage($"Le bonus roleplay de {ctx.oTarget.Name.ColorString(Color.WHITE)} est de {player.bonusRolePlay.ToString().ColorString(Color.WHITE)}", Color.CYAN);
          }
        }
      }
      else
      {
        if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
          ctx.oSender.SendServerMessage($"Votre bonus roleplay est de {player.bonusRolePlay.ToString().ColorString(Color.WHITE)}", Color.CYAN);
      }
    }
  }
}
