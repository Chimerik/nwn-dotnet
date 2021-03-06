﻿using System.Collections.Generic;
using Anvil.API;
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
        if (ctx.oTarget != null && !PlayerSystem.Players.TryGetValue(ctx.oTarget.LoginCreature, out PlayerSystem.Player player))
        {
          if (((string)options.positional[0]).Length == 0)
          {
            ctx.oSender.SendServerMessage($"Le bonus roleplay de {ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)} est de {player.bonusRolePlay.ToString().ColorString(ColorConstants.White)}", ColorConstants.Pink);
          }
          else
          {
            if (int.TryParse((string)options.positional[0], out int iBRP))
            {
              if (iBRP > -1 && iBRP < 5)
              {
                player.bonusRolePlay = iBRP;
                ctx.oSender.SendServerMessage($"Le bonus roleplay de {ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)} est de {player.bonusRolePlay.ToString().ColorString(ColorConstants.White)}", ColorConstants.Pink);
                ctx.oTarget.SendServerMessage($"Votre bonus roleplay est désormais de {player.bonusRolePlay.ToString().ColorString(ColorConstants.White)}", ColorConstants.Pink);

                SqLiteUtils.UpdateQuery("PlayerAccounts",
                  new List<string[]>() { new string[] { "bonusRolePlay", player.bonusRolePlay.ToString() } },
                  new List<string[]>() { new string[] { "rowid", player.accountId.ToString() } });
              }
              else
                ctx.oSender.SendServerMessage("Le bonus roleplay doit être compris entre 0 et 4", ColorConstants.Orange);
            }
            else
              ctx.oSender.SendServerMessage($"Le bonus roleplay de {ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)} est de {player.bonusRolePlay.ToString().ColorString(ColorConstants.White)}", ColorConstants.Cyan);
          }
        }
      }
      else
      {
        if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
          ctx.oSender.SendServerMessage($"Votre bonus roleplay est de {player.bonusRolePlay.ToString().ColorString(ColorConstants.White)}", ColorConstants.Cyan);
      }
    }
  }
}
