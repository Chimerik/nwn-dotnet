using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OrbeChromatique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      var vfx = spell.Id switch
      {
        CustomSpell.OrbeChromatiqueAcide => VfxType.ImpAcidS,
        CustomSpell.OrbeChromatiqueFoudre => VfxType.ImpLightningS,
        CustomSpell.OrbeChromatiqueFroid => VfxType.ImpFrostS,
        CustomSpell.OrbeChromatiquePoison => VfxType.ImpPoisonS,
        _ => VfxType.ImpFlameS,
      };

      foreach (var target in targets)
      {
        if (oCaster is NwCreature caster)
        {
          if (feat is not null && feat.Id == CustomSkill.MonkSphereDequilibreElementaire)
          {
            caster.IncrementRemainingFeatUses(feat.FeatType);
            FeatUtils.DecrementKi(caster, 2);
            castingClass = NwClass.FromClassId(CustomClass.Monk);
          }

          if (castingClass.Id != CustomClass.Monk)
          {
            if (caster.Gold < 50)
            {
              caster.LoginPlayer?.SendServerMessage("Vous devez être en possession de 50 po pour lancer ce sort", ColorConstants.Red);
              return;
            }

            caster.Gold -= 50;
          }
        }

        int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(vfx));

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, castingClass.SpellCastingAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
          case TouchAttackResult.Hit: break;
          default: continue;
        }

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(castingClass));
      }
    }
  }
}
