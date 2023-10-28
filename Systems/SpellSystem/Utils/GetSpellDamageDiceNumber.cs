
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSpellDamageDiceNumber(NwCreature caster, NwSpell spell)
    {

      if (spell.InnateSpellLevel < 1)
      {
        if (caster.Level > 16)
          return 4;
        else if (caster.Level > 10)
          return 3;
        else if (caster.Level > 4)
          return 2;
        else
          return 1;
      }

      return 1;
    }
  }
}
