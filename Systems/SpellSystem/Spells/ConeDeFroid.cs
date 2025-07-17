using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ConeDeFroid(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation)
    {     
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 18, false, oCaster.Location.Position))
      {
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, 5, 
          CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry));
      }
    }
  }
}
