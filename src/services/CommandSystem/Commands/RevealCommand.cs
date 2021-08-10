using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRevealCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oTarget == null)
        RevealPlugin.SetRevealToParty(ctx.oSender.ControlledCreature, 1, RevealPlugin.NWNX_REVEAL_SEEN);
      else
        RevealPlugin.RevealTo(ctx.oSender.ControlledCreature, ctx.oTarget.ControlledCreature, RevealPlugin.NWNX_REVEAL_SEEN);
    }
  }
}
