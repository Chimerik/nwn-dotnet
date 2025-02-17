using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FusionAvecLaPierre(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
    }
  }
}
