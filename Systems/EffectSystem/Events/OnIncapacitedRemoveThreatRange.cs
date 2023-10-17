using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnIncapacitatedRemoveThreatRange(OnEffectApply onEffect)
    {
      if (!EffectUtils.IsIncapacitatingEffect(onEffect.Effect.EffectType) || onEffect.Object is not NwCreature creature)
        return;

      creature.RemoveEffect(threatAoE);
    }
  }
}
