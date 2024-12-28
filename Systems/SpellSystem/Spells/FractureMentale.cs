
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FractureMentale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);
      bool tourPuissant = caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants);

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry);
      
      if (tourPuissant || saveResult == SavingThrowResult.Failure)
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell),
          oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), saveResult, damageDice: target.HP < target.MaxHP ? 12 : 0);

      if (saveResult == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS));
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.FractureMentale, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }
    }
  }
}
