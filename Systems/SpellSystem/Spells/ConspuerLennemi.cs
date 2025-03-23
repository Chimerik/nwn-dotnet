using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ConspuerLennemi(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      int spellDC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Charisma);

      foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC) == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFearS));
        EffectSystem.ApplyEffroi(target, caster, NwTimeSpan.FromRounds(spellEntry.duration), spellDC, true);
      }

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.PaladinConspuerEnnemi);
      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
