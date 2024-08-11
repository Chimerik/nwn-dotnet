using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MauvaisAugure(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);
      SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC);

      if (saveResult == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.MauvaisAugure, NwTimeSpan.FromRounds(2));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseNegative));
      }
      else
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitNegative));

      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, 0, saveResult);
    }
  }
}
