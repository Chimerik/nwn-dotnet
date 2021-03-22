using NWN.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteCommendCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ObjectPlugin.GetInt(ctx.oSender, "_BRP") == 4)
      {
        if (ctx.oTarget != null)
        {
          int iBRP = ObjectPlugin.GetInt(ctx.oTarget, "_BRP");
          if (iBRP < 4)
          {
            ctx.oTarget.SendServerMessage("Un joueur vient de vous recommander pour une augmentation de bonus roleplay !", Color.ROSE);

            if (iBRP == 1)
            {
              ObjectPlugin.SetInt(ctx.oTarget, "_BRP", 2, 1);
              ctx.oTarget.SendServerMessage("Votre bonus roleplay est désormais de 2", Color.GREEN);
            }
            
            Utils.LogMessageToDMs($"{ctx.oSender.Name} vient de recommander {ctx.oTarget.Name} pour une augmentation de bonus roleplay.");
          }
          ctx.oSender.SendServerMessage($"Vous venez de recommander {ctx.oTarget.Name} pour une augmentation de bonus roleplay !", Color.ROSE);
        }
      }
    }
  }
}
