using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRevealCommand(ChatSystem.Context chatContext)
    {
      if (!NWScript.GetIsObjectValid(chatContext.oTarget))
        NWNX.Reveal.RevealToParty(chatContext.oSender, 1, DetectionMethod.Seen);
      else
        NWNX.Reveal.RevealTo(chatContext.oSender, chatContext.oTarget, DetectionMethod.Seen);
    }
  }
}
