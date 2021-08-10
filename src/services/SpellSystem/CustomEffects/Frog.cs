using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class Frog
  {
    public static void ApplyFrogEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetObjectVariable<LocalVariableFloat>("CUSTOM_EFFECT_FROG").HasValue)
        return;

      oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_FROG").Value = (int)oTarget.CreatureAppearanceType;

      oTarget.CreatureAppearanceType = (AppearanceType)6396;
      oTarget.OnSpellCast -= FrogSpellMalus;
      oTarget.OnCreatureDamage -= FrogMalus;
      oTarget.OnSpellCastAt -= FrogMalusCure;
      oTarget.OnCreatureDamage += FrogMalus;
      oTarget.OnSpellCastAt += FrogMalusCure;
      oTarget.OnSpellCast += FrogSpellMalus;
    }
    public static void RemoveFrogEffectFromTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_FROG").HasValue)
        oTarget.CreatureAppearanceType = (AppearanceType)oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_FROG").Value;

      oTarget.OnCreatureDamage -= FrogMalus;
      oTarget.OnSpellCastAt -= FrogMalusCure;
      oTarget.OnSpellCast -= FrogSpellMalus;
    }
    private static void FrogMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
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
    private static void FrogMalus(OnCreatureDamage onDamage)
    {
      int damage = onDamage.DamageData.Base / 4;
      if (damage < 1)
        damage = 1;
      onDamage.DamageData.Base = damage;
    }
    private static void FrogSpellMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:
          return;
      }

      onSpellCast.PreventSpellCast = true;

      if(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
        oPC.ControllingPlayer.SendServerMessage("La métamorphose vous empêche de faire usage de magie !");
    }
  }
}
