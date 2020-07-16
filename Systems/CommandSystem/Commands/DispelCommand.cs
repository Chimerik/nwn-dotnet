using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelCommand(ChatSystem.ChatEventArgs e, Options.Result options)
    {
      if (NWScript.GetIsObjectValid(e.oTarget))
      {
        foreach (Effect eff in e.oTarget.Effects)
        {
          if (NWScript.GetEffectCreator(eff) == e.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(e.oTarget, eff);
        }
      }
      else
      {
        foreach (Effect eff in e.oSender.Effects)
        {
          if (NWScript.GetEffectCreator(eff) == e.oSender && NWScript.GetEffectTag(eff) == "")
            NWScript.RemoveEffect(e.oSender, eff);
        }
      }
    }
  }
}
