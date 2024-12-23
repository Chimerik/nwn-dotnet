
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Message(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
    }
  }
}
