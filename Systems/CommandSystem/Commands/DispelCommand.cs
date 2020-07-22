namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsObjectValid(ctx.oTarget))
      {
        foreach (Effect eff in ctx.oTarget.Effects)
        {
          if (NWScript.GetEffectCreator(eff) == ctx.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(ctx.oTarget, eff);
        }
      }
      else
      {
        foreach (Effect eff in ctx.oSender.Effects)
        {
          if (NWScript.GetEffectCreator(eff) == ctx.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(ctx.oSender, eff);
        }
      }
    }
  }
}
