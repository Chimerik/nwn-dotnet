using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnIncapacitatedRemoveThreatRange(OnEffectApply onEffect)
    {
      if (onEffect.Object is not NwCreature creature || !EffectUtils.IsCannotThreatenEffect(onEffect.Effect.EffectType))
        return;

      creature.RemoveEffect(threatAoE);
    }
  }
}
