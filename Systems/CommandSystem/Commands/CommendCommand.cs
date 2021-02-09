using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteCommendCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ObjectPlugin.GetInt(ctx.oSender, "_BRP") == 4)
      {
        if (NWScript.GetIsObjectValid(ctx.oTarget) == 1)
        {
          int iBRP = ObjectPlugin.GetInt(ctx.oTarget, "_BRP");
          if (iBRP < 4)
          {
            NWScript.SendMessageToPC(ctx.oTarget, $"Un joueur vient de vous recommander pour une augmentation de bonus roleplay !");

            if (iBRP == 1)
            {
              ObjectPlugin.SetInt(ctx.oTarget, "_BRP", 2, 1);
              NWScript.SendMessageToPC(ctx.oTarget, "Votre bonus roleplay est désormais de 2");
            }
            
            NWN.Utils.LogMessageToDMs($"{NWScript.GetName(ctx.oSender)} vient de recommander {NWScript.GetName(ctx.oTarget)} pour une augmentation de bonus roleplay.");
          }

          NWScript.SendMessageToPC(ctx.oSender, $"Vous venez de recommander {NWScript.GetName(ctx.oTarget)} pour une augmentation de bonus roleplay !");
        }
      }
    }
  }
}
