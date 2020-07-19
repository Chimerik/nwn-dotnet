using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRevealCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (!NWScript.GetIsObjectValid(ctx.oTarget))
        NWNX.Reveal.RevealToParty(ctx.oSender, 1, DetectionMethod.Seen);
      else
        NWNX.Reveal.RevealTo(ctx.oSender, ctx.oTarget, DetectionMethod.Seen);
    }
  }
}
