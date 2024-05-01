using System.Linq;
using Anvil.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static void RemoveEffectType(NwGameObject target, params EffectType[] effectType)
    {
      foreach (var eff in target.ActiveEffects)
        if (effectType.Contains(eff.EffectType))
          target.RemoveEffect(eff);
    }
  }
}
