using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Serenity(SpellEvents.OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));

      string taggedEffect = "";

      foreach (var eff in caster.ActiveEffects)
      {
        if (string.IsNullOrEmpty(taggedEffect))
        {
          if(eff.EffectType == EffectType.Charmed || eff.Tag == EffectSystem.CharmEffectTag
            || eff.EffectType == EffectType.Frightened || eff.Tag == EffectSystem.FrightenedEffectTag)
          {
            taggedEffect = eff.Tag;
            caster.RemoveEffect(eff);
          }
        }
        else if(taggedEffect == eff.Tag)
          caster.RemoveEffect(eff);
      }
    }
  }
}
