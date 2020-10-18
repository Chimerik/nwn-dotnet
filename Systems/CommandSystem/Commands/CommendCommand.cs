using System;
using System.Collections.Generic;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteCommendCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ObjectPlugin.GetInt(ctx.oSender, "_BRP") == 4)
      {
        if (ctx.oTarget.IsValid)
        {
          int iBRP = ObjectPlugin.GetInt(ctx.oTarget, "_BRP");
          if (iBRP < 4)
          {
            NWScript.SendMessageToPC(ctx.oTarget, $"Un joueur vient de vous recommander pour une augmentation de bonus roleplay !");

            if (iBRP == 1)
            {
              ObjectPlugin.SetInt(ctx.oTarget, "_BRP", 2, true);
              NWScript.SendMessageToPC(ctx.oTarget, "Votre bonus roleplay est désormais de 2");
            }

            Utils.LogMessageToDMs($"{ctx.oSender.Name} vient de recommander {ctx.NWScript.GetName(oTarget.oid)} pour une augmentation de bonus roleplay.");
          }

          NWScript.SendMessageToPC(ctx.oSender, $"Vous venez de recommander {ctx.NWScript.GetName(oTarget.oid)} pour une augmentation de bonus roleplay !");
        }
      }
    }
  }
}
