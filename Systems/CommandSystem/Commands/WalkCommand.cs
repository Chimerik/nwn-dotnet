using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteWalkCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWNX.Object.GetInt(ctx.oSender, "_ALWAYS_WALK") == 0)
      {
        NWNX.Player.SetAlwaysWalk(ctx.oSender, true);
        NWNX.Object.SetInt(ctx.oSender, "_ALWAYS_WALK", 1, true);
        NWScript.SendMessageToPC(ctx.oSender, "Vous avez activé le mode marche.");
      }
      else
      {
        NWNX.Player.SetAlwaysWalk(ctx.oSender, false);
        NWNX.Object.DeleteInt(ctx.oSender, "_ALWAYS_WALK");
        NWScript.SendMessageToPC(ctx.oSender, "Vous avez désactivé le mode marche.");
      }
    }
  }
}
