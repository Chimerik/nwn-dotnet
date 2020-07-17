using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteWalkCommand(ChatSystem.Context chatContext)
    {
      if (NWNX.Object.GetInt(chatContext.oSender, "_ALWAYS_WALK") == 0)
      {
        NWNX.Player.SetAlwaysWalk(chatContext.oSender, true);
        NWNX.Object.SetInt(chatContext.oSender, "_ALWAYS_WALK", 1, true);
        NWScript.SendMessageToPC(chatContext.oSender, "Vous avez activé le mode marche.");
      }
      else
      {
        NWNX.Player.SetAlwaysWalk(chatContext.oSender, false);
        NWNX.Object.DeleteInt(chatContext.oSender, "_ALWAYS_WALK");
        NWScript.SendMessageToPC(chatContext.oSender, "Vous avez désactivé le mode marche.");
      }
    }
  }
}
