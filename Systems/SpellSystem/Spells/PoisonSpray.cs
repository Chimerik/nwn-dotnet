﻿using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PoisonSpray(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, SpellUtils.GetSpellCastAbility(oCaster, castingClass, feat));

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
        {
          targetCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));

          SavingThrowResult saveResult = CreatureUtils.GetSavingThrowResult(targetCreature, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry, effectType: SpellConfig.SpellEffectType.Poison);

          if (saveResult == SavingThrowResult.Failure || oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants))
            SpellUtils.DealSpellDamage(targetCreature, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(castingClass), saveResult, casterClass: castingClass);
        }
      }
    }
  }
}
