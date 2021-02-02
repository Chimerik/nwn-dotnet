using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelDiseaseCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectDisease(0)), ctx.oSender))
        Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectDisease(0)), ctx.oSender);
    }
  }
}
