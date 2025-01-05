using Anvil.API;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Aide(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry);
      
      foreach (var target in targets)
      {
        if (target is not NwCreature creature)
          continue;

        EffectUtils.RemoveTaggedEffect(target, EffectSystem.AideEffectTag);

        if(creature.IsLoginPlayerCharacter)
        {
          creature.LevelInfo[0].HitDie += 5;
        }
        else
        {
          target.MaxHP += 5;
        }

        target.ApplyEffect(EffectDuration.Instant, Effect.Heal(5));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Aide, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }
    }
  }
}
