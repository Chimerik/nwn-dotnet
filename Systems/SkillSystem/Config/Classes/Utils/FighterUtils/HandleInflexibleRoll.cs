using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static int HandleInflexible(NwCreature creature, int saveRoll)
    {
      int? fighterLevel = creature.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter))?.Level;

      if (!fighterLevel.HasValue || fighterLevel.Value < 9)
        return saveRoll;

      return NwRandom.Roll(Utils.random, 20) - fighterLevel.Value < 13 ? 2 : fighterLevel.Value < 17 ? 1 : 0;
    }
  }
}
