using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFrostAttackCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (
        NWScript.GetLevelByClass(NWScript.CLASS_TYPE_WIZARD, ctx.oSender) > 0 ||
        NWScript.GetLevelByClass(NWScript.CLASS_TYPE_SORCERER, ctx.oSender) > 0
      )
      {
        if (ObjectPlugin.GetInt(ctx.oSender, "_FROST_ATTACK") == 0)
        {
          ObjectPlugin.SetInt(ctx.oSender, "_FROST_ATTACK", 1, 1);
          NWScript.SendMessageToPC(ctx.oSender, "Vous activez le mode d'attaque par rayon de froid");
        }
        else
        {
          ObjectPlugin.DeleteInt(ctx.oSender, "_FROST_ATTACK");
          NWScript.SendMessageToPC(ctx.oSender, "Vous désactivez le mode d'attaque par rayon de froid");
        }
      }
      else
        NWScript.SendMessageToPC(ctx.oSender, "Il vous faut pouvoir lancer le sort rayon de froid pour activer ce mode.");
    }
  }
}
