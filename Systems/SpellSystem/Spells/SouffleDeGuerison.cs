using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SouffleDeGuerison(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
        {
          NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Instant, 
            Effect.Heal(SpellUtils.GetHealAmount(caster, targetCreature, spell, spellEntry, castingClass, spellEntry.numDice))));

          targetCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));
        } 
      }
    }
  }
}
