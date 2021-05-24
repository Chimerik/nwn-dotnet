using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  static class NoHealMagic
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoHealingSpellMalus;
      oTarget.OnSpellCast += NoHealingSpellMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoHealingSpellMalus;
    }
    private static void NoHealingSpellMalus(OnSpellCast onSpellCast)
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

          if (onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
            oPC.ControllingPlayer.SendServerMessage("L'interdiction d'usage de magie curative est en vigueur.", Color.RED);
          break;
      }
    }
  }
}
