using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Telekinesie(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
    }
  }
}
