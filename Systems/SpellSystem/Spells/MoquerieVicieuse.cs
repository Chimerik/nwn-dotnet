﻿using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MoquerieVicieuse(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, SpellUtils.GetSpellCastAbility(oCaster, casterClass, feat));

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
        {
          SavingThrowResult saveResult = CreatureUtils.GetSavingThrowResult(targetCreature, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry);

          if (saveResult == SavingThrowResult.Failure || oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants))
          {
            SpellUtils.DealSpellDamage(targetCreature, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), saveResult);
            NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.MoquerieVicieuse, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
          }
        }
      }
    }
  }
}
