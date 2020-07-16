using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteWalkCommand(ChatSystem.ChatEventArgs e, Options.Result options)
    {
      if (NWNX.Object.GetInt(e.oSender, "_ALWAYS_WALK") == 0)
      {
        NWNX.Player.SetAlwaysWalk(e.oSender, true);
        NWNX.Object.SetInt(e.oSender, "_ALWAYS_WALK", 1, true);
        NWScript.SendMessageToPC(e.oSender, "Vous avez activé le mode marche.");
      }
      else
      {
        NWNX.Player.SetAlwaysWalk(e.oSender, false);
        NWNX.Object.DeleteInt(e.oSender, "_ALWAYS_WALK");
        NWScript.SendMessageToPC(e.oSender, "Vous avez désactivé le mode marche.");
      }
    }
  }
}
