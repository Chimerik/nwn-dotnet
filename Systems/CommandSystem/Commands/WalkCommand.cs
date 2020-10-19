using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteWalkCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ObjectPlugin.GetInt(ctx.oSender, "_ALWAYS_WALK") == 0)
      {
        PlayerPlugin.SetAlwaysWalk(ctx.oSender, 1);
        ObjectPlugin.SetInt(ctx.oSender, "_ALWAYS_WALK", 1, 1);
        NWScript.SendMessageToPC(ctx.oSender, "Vous avez activé le mode marche.");
      }
      else
      {
        PlayerPlugin.SetAlwaysWalk(ctx.oSender, 0);
        ObjectPlugin.DeleteInt(ctx.oSender, "_ALWAYS_WALK");
        NWScript.SendMessageToPC(ctx.oSender, "Vous avez désactivé le mode marche.");
      }
    }
  }
}
