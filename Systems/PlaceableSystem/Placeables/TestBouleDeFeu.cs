using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void TestBouleDeFeu(PlaceableEvents.OnLeftClick onUsed)
    {
      NwSpell burningHands = NwSpell.FromSpellType(Spell.BurningHands);
      var spellEntry = Spells2da.spellTable[burningHands.Id];

      SpellUtils.SignalEventSpellCast(onUsed.ClickedBy.ControlledCreature, onUsed.Placeable, burningHands.SpellType);
      int spellDC = 12;
      int damageDice = SpellUtils.GetSpellDamageDiceNumber(onUsed.Placeable, NwSpell.FromSpellType(Spell.BurningHands));

      //onUsed.Placeable.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.))

      foreach (NwCreature target in onUsed.ClickedBy.ControlledCreature.Location.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, onUsed.ClickedBy.ControlledCreature.Location.Position))
      {
        SpellUtils.DealSpellDamage(target,10, spellEntry, damageDice, onUsed.Placeable, 1,
          CreatureUtils.GetSavingThrow(onUsed.Placeable, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }
    }
  }
}
