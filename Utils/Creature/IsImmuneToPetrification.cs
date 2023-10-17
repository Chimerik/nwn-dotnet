using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool IsImmuneToPetrification(NwCreature creature)
    {
      if (creature.CheckResistSpell(creature) == ResistSpellResult.ResistedMagicImmune)
        return true;

      switch (creature.Race.RacialType)
      {
        case RacialType.Undead:
        case RacialType.Elemental:
        case RacialType.Construct:
          return true;
      }

      switch (creature.Appearance.RowIndex)
      {
        case 369: // Basilisk
        case 368: // Cockatrice
        case 2202: case 2203: case 352: // Medusa
          return true;
      }
     
      if (creature.PlotFlag || creature.IsDMAvatar)
        return true;
      
      return false;
    }
  }
}
