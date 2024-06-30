using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Scrutation(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
    }
  }
}
