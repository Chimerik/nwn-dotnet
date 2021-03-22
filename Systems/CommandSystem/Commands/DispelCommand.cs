using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oTarget != null)
        foreach (API.Effect effect in ctx.oTarget.ActiveEffects.Where(e => e.Creator == ctx.oSender))
          ctx.oTarget.RemoveEffect(effect);
      else
        foreach (API.Effect effect in ctx.oSender.ActiveEffects.Where(e => e.Creator == ctx.oSender))
          ctx.oSender.RemoveEffect(effect);
    }
  }
}
