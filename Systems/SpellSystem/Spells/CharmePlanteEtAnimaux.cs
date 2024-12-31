using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CharmePlanteEtAnimaux(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Wisdom);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosHoly10));

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if(target.Race.RacialType == RacialType.Animal && target.Master is null && caster.IsReactionTypeHostile(target) && !EffectSystem.IsCharmeImmune(caster, target)
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC) == SavingThrowResult.Failure)
        {
          EffectSystem.ApplyCharme(target, caster, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.ClercCharmePlanteEtAnimaux);
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
