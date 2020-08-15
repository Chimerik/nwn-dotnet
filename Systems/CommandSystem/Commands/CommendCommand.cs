using System;
using System.Collections.Generic;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteCommendCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWNX.Object.GetInt(ctx.oSender, "_BRP") == 4)
      {
        if (ctx.oTarget.IsValid)
        {
          int iBRP = NWNX.Object.GetInt(ctx.oTarget, "_BRP");
          if (iBRP < 4)
          {
            NWScript.SendMessageToPC(ctx.oTarget, $"Un joueur vient de vous recommander pour une augmentation de bonus roleplay !");

            if (iBRP == 1)
            {
              NWNX.Object.SetInt(ctx.oTarget, "_BRP", 2, true);
              NWScript.SendMessageToPC(ctx.oTarget, "Votre bonus roleplay est désormais de 2");
            }

            Utils.LogMessageToDMs($"{ctx.oSender.Name} vient de recommander {ctx.oTarget.Name} pour une augmentation de bonus roleplay.");
          }

          NWScript.SendMessageToPC(ctx.oSender, $"Vous venez de recommander {ctx.oTarget.Name} pour une augmentation de bonus roleplay !");
        }
      }
    }
  }
}
