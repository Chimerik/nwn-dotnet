using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class NoHealMagic
  {
    public NoHealMagic(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoHealingSpellMalus;
      oTarget.OnSpellCast += NoHealingSpellMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoHealingSpellMalus;
    }
    private void NoHealingSpellMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.Heal:
        case Spell.HealingCircle:
        case Spell.MassHeal:
        case Spell.CureCriticalWounds:
        case Spell.CureLightWounds:
        case Spell.CureMinorWounds:
        case Spell.CureModerateWounds:
        case Spell.CureSeriousWounds:
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveBlindnessAndDeafness:
        case Spell.RemoveCurse:
        case Spell.RemoveDisease:
        case Spell.RemoveFear:
        case Spell.RemoveParalysis:
        case Spell.NaturesBalance:
        case Spell.NeutralizePoison:
        case Spell.Regenerate:
        case Spell.MonstrousRegeneration:
          onSpellCast.PreventSpellCast = true;
          ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de magie curative est en vigueur.", Color.RED);
          break;
      }
    }
  }
}
