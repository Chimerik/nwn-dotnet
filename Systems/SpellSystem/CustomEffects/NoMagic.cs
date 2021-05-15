using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class NoMagic
  {
    public NoMagic(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= SpellSystem.HandleBeforeSpellCast;
      oTarget.OnSpellCast -= NoMagicMalus;
      oTarget.OnSpellCast += NoMagicMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      oTarget.OnSpellCast -= NoMagicMalus;
    }
    private void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;
      ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de sorts est en vigueur.", Color.RED);
    }
  }
}
