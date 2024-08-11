using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PoisonSpray(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));

      SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry, effectType: SpellConfig.SpellEffectType.Poison);

      if(saveResult == SavingThrowResult.Failure || oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants))
        SpellUtils.DealSpellDamage(oTarget, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(castingClass), saveResult);
    }
  }
}
