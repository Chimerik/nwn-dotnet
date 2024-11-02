
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Quete(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
    }
  }
}
