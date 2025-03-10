using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SouffleDeGuerisonDeGroupe(NwGameObject oCaster, NwGameObject oTarget, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry);

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature && !Utils.In(targetCreature.Race.RacialType, RacialType.Undead, RacialType.Construct))
        {
          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, 
            Effect.Heal(SpellUtils.GetHealAmount(caster, targetCreature, spell, spellEntry, castingClass, spellEntry.numDice))));

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));
        }
      }
    }  
  }
}
