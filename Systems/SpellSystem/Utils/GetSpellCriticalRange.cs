using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSpellCriticalRange(NwCreature creature)
    {
      byte? championLevel = creature.Classes.FirstOrDefault(c => c.Class.Id == CustomClass.Champion)?.Level;

      if (!championLevel.HasValue || championLevel.Value < 3)
        return 20;

      return championLevel.Value < 15 ? 19 : 18;
    }
  }
}
