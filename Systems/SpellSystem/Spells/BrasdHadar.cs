
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BrasdHadar(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (target == oCaster)
          continue;

        SavingThrowResult result = CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry);

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass),
          result);

        if (result == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(EffectSystem.noReactions(target), Effect.VisualEffect(VfxType.DurTentacle), Effect.Icon(CustomEffectIcon.BrasDhadar)), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }
    }
  }
}
