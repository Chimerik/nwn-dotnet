using Anvil.API;
using NWN.Core;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> RayonAffaiblissant(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster)
        return targetList;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        if (target is not NwCreature targetCreature)
          continue;

        if(CreatureUtils.GetSavingThrowResult(targetCreature, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.RayonAffaiblissant(spell, casterClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry));
          targetList.Add(targetCreature);
        }
        else
        {
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.RayonAffaiblissantDesavantage, NwTimeSpan.FromRounds(1));
          EffectSystem.ApplyPoison(targetCreature, caster, NwTimeSpan.FromRounds(spellEntry.duration), spellEntry.savingThrowAbility);
        }

        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamEvil, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7)));
      }

      return targetList;
    }
  }
}
