using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CouleursDansantes(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      Effect eff = Effect.LinkEffects(Effect.Blindness(), Effect.VisualEffect(VfxType.DurCessateNegative), Effect.VisualEffect(VfxType.DurMindAffectingNegative), Effect.VisualEffect(VfxType.DurBlind));

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, oCaster.Location.Position))
      {
        if (target == oCaster)
          continue;

        if(CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpBlindDeafM));
          target.ApplyEffect(EffectDuration.Temporary, eff, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }
    }
  }
}
