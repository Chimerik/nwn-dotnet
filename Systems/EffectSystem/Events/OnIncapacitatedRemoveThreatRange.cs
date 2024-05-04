using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnIncapacitatedRemoveThreatRange(OnEffectApply onEffect)
    {
      if (onEffect.Object is not NwCreature creature || !EffectUtils.IsIncapacitatingEffect(onEffect.Effect))
        return;

      foreach (var eff in creature.ActiveEffects)
        if(eff.Tag == ThreatenedAoETag)
          creature.RemoveEffect(eff);
    }
  }
}
