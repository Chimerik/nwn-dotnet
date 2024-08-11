using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BoneChill(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      switch(SpellUtils.GetSpellAttackRoll(oTarget, oCaster, spell, castingClass.SpellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      oTarget.OnHeal -= PreventHeal;
      oTarget.OnHeal += PreventHeal;
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.boneChillEffect, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      SpellUtils.DealSpellDamage(oTarget, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(castingClass));
    }
  }
}
