using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BoneChill(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      foreach (var target in targets)
      {
        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, castingClass.SpellCastingAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
          case TouchAttackResult.Hit: break;
          default: return;
        }

        target.OnHeal -= PreventHeal;
        target.OnHeal += PreventHeal;

        if(oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.DechargeRepulsive))
          target.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(50), NwTimeSpan.FromRounds(1));

        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.boneChillEffect, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(castingClass), casterClass: castingClass);
      }
    }
  }
}
