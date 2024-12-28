using Anvil.API;
namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSpellRange(NwSpell spell)
    {
      switch(spell.Range)
      {
        case SpellRange.Short: return 81;
        case SpellRange.Medium: return 324;
        case SpellRange.Long: return 1296;

        case SpellRange.Personal:
        case SpellRange.Touch:
        default: return 9;
      }
    }
  }
}
