using NWN.API.Constants;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelDiseaseCommand(ChatSystem.Context ctx, Options.Result options)
    {
      foreach (API.Effect eff in ctx.oSender.ActiveEffects.Where(e => e.EffectType == EffectType.Disease))
          ctx.oSender.RemoveEffect(eff);
    }
  }
}
