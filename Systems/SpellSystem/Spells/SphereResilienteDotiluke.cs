using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SphereResilienteDotiluke(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is NwCreature caster)
        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");
    }
  }
}
