using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class Mini
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetObjectVariable<LocalVariableFloat>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").HasValue)
        return;

      oTarget.GetObjectVariable<LocalVariableFloat>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").Value = oTarget.VisualTransform.Scale;
      oTarget.VisualTransform.Scale *= 0.6f;

      oTarget.OnCreatureDamage -= MiniMalus;
      oTarget.OnSpellCastAt -= MiniMalusCure;
      oTarget.OnCreatureDamage += MiniMalus;
      oTarget.OnSpellCastAt += MiniMalusCure;
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      if (oTarget.GetObjectVariable<LocalVariableFloat>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").HasValue)
      {
        oTarget.VisualTransform.Scale = oTarget.GetObjectVariable<LocalVariableFloat>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").Value;
        oTarget.GetObjectVariable<LocalVariableFloat>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").Delete();
      }

      oTarget.OnCreatureDamage -= MiniMalus;
      oTarget.OnSpellCastAt -= MiniMalusCure;
    }
    private static void MiniMalus(OnCreatureDamage onDamage)
    {
      onDamage.DamageData.Base = 1;
    }
    private static void MiniMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell.SpellType)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:

          foreach (Effect miniMalus in onSpellCastAt.Creature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_MINI"))
            onSpellCastAt.Creature.RemoveEffect(miniMalus);

          break;
      }
    }
  }
}
