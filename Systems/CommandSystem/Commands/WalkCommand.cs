using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteWalkCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ObjectPlugin.GetInt(ctx.oSender, "_ALWAYS_WALK") == 0)
      {
        PlayerPlugin.SetAlwaysWalk(ctx.oSender, true);
        ObjectPlugin.SetInt(ctx.oSender, "_ALWAYS_WALK", 1, true);
        NWScript.SendMessageToPC(ctx.oSender, "Vous avez activé le mode marche.");
      }
      else
      {
        PlayerPlugin.SetAlwaysWalk(ctx.oSender, false);
        ObjectPlugin.DeleteInt(ctx.oSender, "_ALWAYS_WALK");
        NWScript.SendMessageToPC(ctx.oSender, "Vous avez désactivé le mode marche.");
      }
    }
  }
}
