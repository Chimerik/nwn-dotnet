using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Deguisement(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      // TODO : dans le futur, peut-être ajouter un coup de NWN_Rename ?
    }
  }
}
