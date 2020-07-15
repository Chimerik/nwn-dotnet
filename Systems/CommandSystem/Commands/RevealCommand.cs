using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRevealCommand(ChatSystem.ChatEventArgs e)
    {
      if (!NWScript.GetIsObjectValid(e.oTarget))
        NWNX.Reveal.RevealToParty(e.oSender, 1, DetectionMethod.Seen);
      else
        NWNX.Reveal.RevealTo(e.oSender, e.oTarget, DetectionMethod.Seen);
    }
  }
}
