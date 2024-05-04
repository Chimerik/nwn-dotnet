using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnIncapacitatedRemoveConcentration(OnEffectApply onEffect)
    {
      if (onEffect.Object is not NwCreature creature || !EffectUtils.IsIncapacitatingEffect(onEffect.Effect))
        return;

      SpellUtils.DispelConcentrationEffects(creature);

      foreach (var eff in creature.ActiveEffects)
      {
        if (eff.Tag == DisengageffectTag || eff.Tag == SprintEffectTag)
          continue;

        if (eff.Tag == DodgeEffectTag)
          creature.RemoveEffect(eff);
      }
    }
  }
}
