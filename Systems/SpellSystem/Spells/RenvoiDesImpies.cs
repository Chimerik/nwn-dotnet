using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RenvoiDesImpies(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Charisma);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosHoly10));

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if(Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Outsider)
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSunstrike));
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetRenvoiDesImpiesEffect(target), NwTimeSpan.FromRounds(spellEntry.duration));
        }
      }

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.DevotionRenvoiDesImpies);
      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
