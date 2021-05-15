using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class Mini
  {
    public Mini(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI").HasValue)
        return;

      VisualTransform visualT = oTarget.VisualTransform;
      oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI").Value = oTarget.VisualTransform.Scale;
      visualT.Scale *= 0.6f;
      oTarget.VisualTransform = visualT;

      oTarget.OnCreatureDamage -= MiniMalus;
      oTarget.OnSpellCastAt -= MiniMalusCure;
      oTarget.OnCreatureDamage += MiniMalus;
      oTarget.OnSpellCastAt += MiniMalusCure;
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      if (oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI").HasValue)
      {
        VisualTransform visualT = oTarget.VisualTransform;
        visualT.Scale = oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI").Value;
        oTarget.GetLocalVariable<float>("CUSTOM_EFFECT_MINI").Delete();
        oTarget.VisualTransform = visualT;
      }

      oTarget.OnCreatureDamage -= MiniMalus;
      oTarget.OnSpellCastAt -= MiniMalusCure;
    }
    private void MiniMalus(OnCreatureDamage onDamage)
    {
      onDamage.DamageData.Base = 1;
    }
    private void MiniMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
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
