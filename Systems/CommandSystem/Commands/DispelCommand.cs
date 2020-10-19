using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsObjectValid(ctx.oTarget) == 1)
      {
        var eff = NWScript.GetFirstEffect(ctx.oTarget);
        while (NWScript.GetIsEffectValid(eff) == 1)
        {
          if (NWScript.GetEffectCreator(eff) == ctx.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(ctx.oTarget, eff);
          eff = NWScript.GetNextEffect(ctx.oTarget);
        }
      }
      else
      {
        var eff = NWScript.GetFirstEffect(ctx.oSender);
        while (NWScript.GetIsEffectValid(eff) == 1)
        {
          if (NWScript.GetEffectCreator(eff) == ctx.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(ctx.oSender, eff);
          eff = NWScript.GetNextEffect(ctx.oSender);
        }
      }
    }
  }
}
