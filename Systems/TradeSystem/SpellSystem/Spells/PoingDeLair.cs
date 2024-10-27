using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PoingDeLair(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);
      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
      SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(caster, target, Ability.Strength, spellDC, spellEntry);

      if (saveResult == SavingThrowResult.Failure)
        EffectSystem.ApplyKnockdown(target, CreatureSize.Large, spellEntry.duration);

      if (caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))
        damageDice += 1;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurst));
      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 1);

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.MonkPoingDeLair);
      FeatUtils.DecrementKi(caster, 2);
    }
  }
}
