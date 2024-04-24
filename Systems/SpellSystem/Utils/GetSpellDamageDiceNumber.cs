
using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSpellDamageDiceNumber(NwGameObject oCaster, NwSpell spell)
    {
      if (spell.InnateSpellLevel < 1)
      {
        if (oCaster is NwCreature caster)
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
        else
          return (int)Math.Floor((double)(spell.InnateSpellLevel * 2) - 1);
      }
      else
        return Spells2da.spellTable[spell.Id].numDice; // TODO : Il faudra gérer le upcast
    }
  }
}
