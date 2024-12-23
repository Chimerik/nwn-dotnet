using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CharmePersonne(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {     
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oTarget is not NwCreature targetCreature || oCaster is not NwCreature caster)
        return;

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(caster, oTarget, spellEntry, true);
      int DC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (var targetObject in targets)
      {
        if(targetObject is NwCreature target && !EffectSystem.IsCharmeImmune(caster, target) 
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC, spellEntry) == SavingThrowResult.Failure)
        {
         EffectSystem.ApplyCharme(target, caster, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }        
    }
  }
}
