using System.Linq;
using Anvil.API;
using NWN.Systems;

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
    public static bool RemoveFirstEffectType(NwGameObject target, EffectType effectType)
    {
      bool effectRemoved = false;

      if(effectType == EffectType.Poison)
      {
        RemoveTaggedEffect(target, EffectSystem.PoisonEffectTag);
      }

      foreach (var eff in target.ActiveEffects)
        if (effectType == eff.EffectType)
        {
          target.RemoveEffect(eff);
          effectRemoved = true;
        }

      return effectRemoved;
    }
  }
}
