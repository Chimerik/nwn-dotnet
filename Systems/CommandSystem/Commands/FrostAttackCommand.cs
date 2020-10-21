using System;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFrostAttackCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (Convert.ToBoolean(NWScript.GetHasSpell(NWScript.SPELL_RAY_OF_FROST, ctx.oSender)))
      {
        PlayerSystem.Player player;
        if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
        {
          if (player.isFrostAttackOn)
          {
            player.isFrostAttackOn = true;
            NWScript.SendMessageToPC(ctx.oSender, "Vous activez le mode d'attaque par rayon de froid");
          }
          else
          {
            player.isFrostAttackOn = false;
            NWScript.SendMessageToPC(ctx.oSender, "Vous désactivez le mode d'attaque par rayon de froid");
          }
        }
      }
      else
        NWScript.SendMessageToPC(ctx.oSender, "Il vous faut pouvoir lancer le sort rayon de froid pour activer ce mode.");
    }
  }
}
