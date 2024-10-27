using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Domination(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster)
        return targetList;

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(caster, oTarget, spellEntry, true);
      int DC = SpellUtils.GetCasterSpellDC(caster, spell, castingClass.SpellCastingAbility);

      foreach (var target in targets)
      {
        if(target is NwCreature targetCreature && CreatureUtils.IsHumanoid(targetCreature) && !EffectSystem.IsCharmeImmune(caster, targetCreature) 
          && CreatureUtils.GetSavingThrow(caster, targetCreature, spellEntry.savingThrowAbility, DC) == SavingThrowResult.Failure)
        {
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Dominated(), NwTimeSpan.FromRounds(spellEntry.duration)));
          targetList.Add(target);
        }
      }

      return targetList;
    }
  }
}
