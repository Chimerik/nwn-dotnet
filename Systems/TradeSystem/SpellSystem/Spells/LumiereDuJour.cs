using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void LumiereDuJour(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is NwCreature caster)
        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");
    }
  }
}
