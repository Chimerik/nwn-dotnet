using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RenvoiDesInfideles(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Charisma);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosHoly10));

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if(Utils.In(target.Race.RacialType, RacialType.Fey, RacialType.Outsider)
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC))
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSunstrike));
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetRenvoiDesImpiesEffect(target), NwTimeSpan.FromRounds(spellEntry.duration));
        }
      }

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
