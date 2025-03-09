using System;
using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void EclatSorcier(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass, NwFeat feat)
    {

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      var vfx = VfxType.ImpFlameS;
      var mirv = VfxType.ImpMirvFlame;

      switch(spell.Id)
      {
        case CustomSpell.EclatSorcierAcide:
          vfx = VfxType.ImpAcidS;
          mirv = VfxType.DurMirvAcid;
          break;

        case CustomSpell.EclatSorcierElec:
          vfx = VfxType.ImpLightningS;
          mirv = VfxType.ImpMirvElectric;
          break;

        case CustomSpell.EclatSorcierTonnerre:
          vfx = VfxType.ImpSonic;
          mirv = VfxType.ImpMirvElectric;
          break;

        case CustomSpell.EclatSorcierFroid:
          vfx = VfxType.ImpFrostS;
          break;

        case CustomSpell.EclatSorcierPoison:
          vfx = VfxType.ImpPoisonS;
          mirv = VfxType.DurMirvAcid;
          break;
      }

    double visualDelay = 0.1;
     var castAbility = SpellUtils.GetSpellCastAbility(oCaster, castingClass, feat);

      foreach (var target in targets)
      {
        int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, castAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
          case TouchAttackResult.Hit: break;
          default: continue;
        }

        double targetDistance = oCaster.Distance(target);
        double damageDelay = (targetDistance / (3.0 * Math.Log(targetDistance) + 2.0)) + visualDelay;
        SpellUtils.DelayMirvDamageImpact(oCaster, target, spell, spellEntry, castingClass, damageDelay, vfx, mirv, nbDice);
        SpellUtils.DelayMirvVisualImpact(oCaster, target, visualDelay, mirv);
        visualDelay += 0.1;
      }
    }
  }
}
