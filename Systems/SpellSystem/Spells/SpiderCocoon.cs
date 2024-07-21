using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SpiderCocoon(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target || oCaster is not NwCreature caster)
        return;
   
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, 8 + NativeUtils.GetCreatureProficiencyBonus(caster.Master) + caster.GetAbilityModifier(Ability.Wisdom)))
      {
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.MauvaisAugure, NwTimeSpan.FromRounds(2));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseNegative));
      }
      else
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitNegative));


    }
  }
}
