using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  static class Mini
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").HasValue)
        return;

      VisualTransform visualT = oTarget.VisualTransform;
      oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").Value = oTarget.VisualTransform.Scale;
      visualT.Scale *= 0.6f;
      oTarget.VisualTransform = visualT;

      oTarget.OnCreatureDamage -= MiniMalus;
      oTarget.OnSpellCastAt -= MiniMalusCure;
      oTarget.OnCreatureDamage += MiniMalus;
      oTarget.OnSpellCastAt += MiniMalusCure;
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      if (oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").HasValue)
      {
        VisualTransform visualT = oTarget.VisualTransform;
        visualT.Scale = oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").Value;
        oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI_INITIAL_SIZE").Delete();
        oTarget.VisualTransform = visualT;
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
      switch (onSpellCastAt.Spell)
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
