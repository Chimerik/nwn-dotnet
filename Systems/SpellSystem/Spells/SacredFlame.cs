using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SacredFlame(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {

      if (oTarget is not NwCreature target)
        return;

      int spellDC = 10;
      bool tourPuissant = false;

      if (oCaster is NwCreature caster)
      {
        spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);
        tourPuissant = caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants);
      }

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();

      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);

      if (advantage < -900)
        return;

      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

      if (saveFailed || tourPuissant)
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), saveFailed);
    }
  }
}
