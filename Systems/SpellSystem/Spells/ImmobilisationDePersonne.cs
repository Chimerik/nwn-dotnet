using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ImmobilisationDePersonne(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass, NwFeat feat = null)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster)
        return targetList;

      if (feat is not null && feat.Id == CustomSkill.MonkPoigneDuVentDuNord)
      {
        caster.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(caster, 3);
        castingClass = NwClass.FromClassId(CustomClass.Monk);
      }

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(caster, oTarget, spellEntry, true);
      int nbTargets = oCaster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;
      int DC = SpellUtils.GetCasterSpellDC(caster, spell, castingClass.SpellCastingAbility);

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature 
          && (CreatureUtils.IsHumanoid(targetCreature))
          && CreatureUtils.GetSavingThrow(caster, targetCreature, spellEntry.savingThrowAbility, DC, spellEntry) == SavingThrowResult.Failure)
        {
          NWScript.AssignCommand(caster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetImmobilisationDePersonneEffect(castingClass.SpellCastingAbility, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
          targetList.Add(target);
        }
      }

      return targetList;
    }
  }
}
