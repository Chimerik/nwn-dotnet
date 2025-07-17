using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BurningHands(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass, Location targetLocation)
    {     
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, oCaster.Location.Position))
      {
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 1, 
            CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry));
      }
    }
  }
}
