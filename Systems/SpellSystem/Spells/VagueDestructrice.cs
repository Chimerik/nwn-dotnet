
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void VagueDestructrice(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {     
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlOdd));

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (oCaster is NwCreature casterCreature && !casterCreature.IsReactionTypeHostile(target))
          continue;

        SavingThrowResult saveResult = CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry);

        if (saveResult == SavingThrowResult.Failure)
          target.ApplyEffect(EffectDuration.Temporary, Effect.Knockdown(), NwTimeSpan.FromRounds(1));

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass), saveResult);
      }
    }
  }
}
