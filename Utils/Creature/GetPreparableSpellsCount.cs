using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetPreparableSpellsCount(PlayerSystem.Player player, NwClass selectedClass)
    {
      int classLevel = player.oid.LoginCreature.GetClassInfo(selectedClass).Level;
      int nbPreparableSpells = player.learnableSpells.Values.Where(s => s.alwaysPrepared).Count();

      switch (selectedClass.Id)
      {
        case CustomClass.Wizard:

          nbPreparableSpells += 3 + classLevel;

          if (classLevel > 8)
            nbPreparableSpells += 1;

          if (classLevel > 11)
            nbPreparableSpells -= 1;

          return nbPreparableSpells;

        case CustomClass.Paladin:
        case CustomClass.Ranger:

          nbPreparableSpells += 2 + classLevel;

          if (classLevel > 5)
            nbPreparableSpells -= 1;

          if (classLevel > 7)
            nbPreparableSpells -= 1;

          if (classLevel > 8)
            nbPreparableSpells += 1;

          if (classLevel > 9)
            nbPreparableSpells -= 1;

          if (classLevel > 11)
            nbPreparableSpells -= 1;

          if (classLevel > 13)
            nbPreparableSpells -= 1;

          if (classLevel > 15)
            nbPreparableSpells -= 1;

          if (classLevel > 17)
            nbPreparableSpells -= 1;

          if (classLevel > 19)
            nbPreparableSpells -= 1;

          return nbPreparableSpells;

        default: // default = Cleric et druid !

          nbPreparableSpells += 3 + classLevel;

          if (classLevel > 8)
            nbPreparableSpells += 1;

          if (classLevel > 11)
            nbPreparableSpells -= 1;

          if (classLevel > 13)
            nbPreparableSpells -= 1;

          if (classLevel > 15)
            nbPreparableSpells -= 1;

          return nbPreparableSpells;
      }
    }
  }
}
