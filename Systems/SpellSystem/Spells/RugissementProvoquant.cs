using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RugissementProvoquant(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlMind));

      foreach (var target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
          EffectSystem.ApplyProvocation(oCaster, target, NwTimeSpan.FromRounds(spellEntry.duration));
      }
    }
  }
}
