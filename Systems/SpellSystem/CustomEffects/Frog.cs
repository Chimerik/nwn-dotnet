using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class Frog
  {
    public Frog(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyFrogEffectToTarget(oTarget);
      else
        RemoveFrogEffectFromTarget(oTarget);
    }
    private void ApplyFrogEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_FROG").HasValue)
        return;

      oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_FROG").Value = (int)oTarget.CreatureAppearanceType;

      oTarget.CreatureAppearanceType = (AppearanceType)6396;
      oTarget.OnSpellCast -= FrogSpellMalus;
      oTarget.OnCreatureDamage -= FrogMalus;
      oTarget.OnSpellCastAt -= FrogMalusCure;
      oTarget.OnCreatureDamage += FrogMalus;
      oTarget.OnSpellCastAt += FrogMalusCure;
      oTarget.OnSpellCast += FrogSpellMalus;
    }
    private void RemoveFrogEffectFromTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_FROG").HasValue)
        oTarget.CreatureAppearanceType = (AppearanceType)oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_FROG").Value;

      oTarget.OnCreatureDamage -= FrogMalus;
      oTarget.OnSpellCastAt -= FrogMalusCure;
      oTarget.OnSpellCast -= FrogSpellMalus;
    }
    private void FrogMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:

          foreach (Effect frogMalus in onSpellCastAt.Creature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_FROG"))
            onSpellCastAt.Creature.RemoveEffect(frogMalus);
          break;
      }
    }
    private void FrogMalus(OnCreatureDamage onDamage)
    {
      int damage = onDamage.DamageData.Base / 4;
      if (damage < 1)
        damage = 1;
      onDamage.DamageData.Base = damage;
    }
    private void FrogSpellMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:
          return;
      }

      onSpellCast.PreventSpellCast = true;
      ((NwPlayer)onSpellCast.Caster).SendServerMessage("La métamorphose vous empêche de faire usage de magie !");
    }
  }
}
