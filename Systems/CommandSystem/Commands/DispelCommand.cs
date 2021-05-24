using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oTarget != null)
        foreach (API.Effect effect in ctx.oTarget.ControlledCreature.ActiveEffects.Where(e => e.Creator == ctx.oSender.ControlledCreature))
          ctx.oTarget.ControlledCreature.RemoveEffect(effect);
      else
        foreach (API.Effect effect in ctx.oSender.ControlledCreature.ActiveEffects.Where(e => e.Creator == ctx.oSender.ControlledCreature))
          ctx.oSender.ControlledCreature.RemoveEffect(effect);
    }
  }
}
