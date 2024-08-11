using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SacredFlame(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oTarget is not NwCreature target)
        return;

      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility); ;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry);

      if(saveResult == SavingThrowResult.Failure || oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants))
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), saveResult);
    }
  }
}
