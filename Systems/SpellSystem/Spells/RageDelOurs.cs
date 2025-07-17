
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RageDelOurs(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      oCaster.ApplyEffect(EffectDuration.Permanent, EffectSystem.RageDelOurs, NwTimeSpan.FromRounds(spellEntry.duration));

      if (oCaster is not NwCreature caster)
        return;

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if(caster.IsReactionTypeHostile(target)
          && CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          EffectSystem.ApplyEffroi(target, caster, NwTimeSpan.FromRounds(1), spellDC);
        }
      }
    }
  }
}
