namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelCommand(ChatSystem.Context chatContext)
    {
      if (NWScript.GetIsObjectValid(chatContext.oTarget))
      {
        foreach (Effect eff in chatContext.oTarget.Effects)
        {
          if (NWScript.GetEffectCreator(eff) == chatContext.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(chatContext.oTarget, eff);
        }
      }
      else
      {
        foreach (Effect eff in chatContext.oSender.Effects)
        {
          if (NWScript.GetEffectCreator(eff) == chatContext.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(chatContext.oSender, eff);
        }
      }
    }
  }
}
