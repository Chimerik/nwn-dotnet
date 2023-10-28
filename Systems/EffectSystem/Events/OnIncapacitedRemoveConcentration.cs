using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnIncapacitatedRemoveConcentration(OnEffectApply onEffect)
    {
      if (onEffect.Object is not NwCreature creature || !EffectUtils.IsIncapacitatingEffect(onEffect.Effect.EffectType))
        return;

      SpellUtils.DispelConcentrationEffects(creature);
    }
  }
}
