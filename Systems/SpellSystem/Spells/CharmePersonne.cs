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

      List<NwCreature> targets = new();
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;
      int DC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      if (nbTargets > 0)
      {
        for (int i = 0; i < nbTargets; i++)
        {
          targets.Add(caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_SPELL_TARGET_{i}").Value);
          caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_SPELL_TARGET_{i}").Delete();
        }

        caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
      }
      else
      {
        targets.Add(targetCreature);
      }

      foreach (var target in targets.Distinct())
      {
        if(!EffectSystem.IsCharmeImmune(caster, target) && 
          !CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC, spellEntry))
        {
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Charme, NwTimeSpan.FromRounds(spellEntry.duration)));
        }
      }        
    }
  }
}
