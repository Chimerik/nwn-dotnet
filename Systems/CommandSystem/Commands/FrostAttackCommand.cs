using NWN.Enums;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFrostAttackCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (
        NWScript.GetLevelByClass(ClassType.Wizard, ctx.oSender) > 0 ||
        NWScript.GetLevelByClass(ClassType.Sorcerer, ctx.oSender) > 0
      )
      {
        if (ObjectPlugin.GetInt(ctx.oSender, "_FROST_ATTACK") == 0)
        {
          ObjectPlugin.SetInt(ctx.oSender, "_FROST_ATTACK", 1, true);
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
