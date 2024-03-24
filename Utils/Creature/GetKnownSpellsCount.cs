using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetKnownSpellsCount(NwCreature creature, CreatureClassInfo classInfo)
    {
      int knownSpells = 0;

      foreach (var spellLevel in classInfo.KnownSpells.Skip(1))
        knownSpells += spellLevel.Count();

      return knownSpells;
    }
  }
}
