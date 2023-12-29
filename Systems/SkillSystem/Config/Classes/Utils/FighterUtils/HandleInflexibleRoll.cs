using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static int HandleInflexible(NwCreature creature, int saveRoll)
    {
      var classes = creature.Classes.Where(c => c.Class.Id == CustomClass.Fighter || c.Class.Id == CustomClass.Champion
      || c.Class.Id == CustomClass.ArcaneArcher || c.Class.Id == CustomClass.Warmaster || c.Class.Id == CustomClass.EldritchKnight);

      int fighterLevel = classes.Sum(c => c.Level);

      if (fighterLevel < 9)
        return saveRoll;

      return NwRandom.Roll(Utils.random, 20) - fighterLevel < 13 ? 2 : fighterLevel < 17 ? 1 : 0;
    }
  }
}
