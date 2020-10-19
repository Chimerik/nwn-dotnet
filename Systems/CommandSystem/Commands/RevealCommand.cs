using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRevealCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsObjectValid(ctx.oTarget) == 1)
        RevealPlugin.SetRevealToParty(ctx.oSender, 1, RevealPlugin.NWNX_REVEAL_SEEN);
      else
        RevealPlugin.RevealTo(ctx.oSender, ctx.oTarget, RevealPlugin.NWNX_REVEAL_SEEN);
    }
  }
}
