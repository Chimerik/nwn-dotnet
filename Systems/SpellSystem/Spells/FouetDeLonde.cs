using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FouetDeLonde(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);
      bool savedFailed = CreatureUtils.GetSavingThrow(caster, target, Ability.Strength, spellDC, spellEntry);
      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      if (caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))
        nbDice += 1;

      if (savedFailed)
        EffectSystem.ApplyKnockdown(target, CreatureSize.Large, spellEntry.duration);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurst));
      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, 1);

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.MonkFouetDeLonde);
      FeatUtils.DecrementKi(caster, 2);
    }
  }
}
