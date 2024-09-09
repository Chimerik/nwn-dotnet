
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Terrassement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Strength);

      oCaster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      _ = oCaster.ClearActionQueue();
      _ = oCaster.ActionJumpToLocation(targetLocation);
      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDustExplosion));

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (oCaster != target 
          && CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
          EffectSystem.ApplyKnockdown(target, CreatureSize.Large, 1);
      }
    }
  }
}
