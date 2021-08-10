using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oTarget != null)
        foreach (Effect effect in ctx.oTarget.ControlledCreature.ActiveEffects.Where(e => e.Creator == ctx.oSender.ControlledCreature))
          ctx.oTarget.ControlledCreature.RemoveEffect(effect);
      else
        foreach (Effect effect in ctx.oSender.ControlledCreature.ActiveEffects.Where(e => e.Creator == ctx.oSender.ControlledCreature))
          ctx.oSender.ControlledCreature.RemoveEffect(effect);
    }
  }
}
