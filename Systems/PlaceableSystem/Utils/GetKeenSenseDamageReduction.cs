using System;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class TrapUtils
  {
    public static int GetKeenSenseDamageReduction(NwCreature creature, int damage)
    {
      if (creature.KnowsFeat(Feat.KeenSense))
      {
        damage /= 2;

        creature?.LoginPlayer.DisplayFloatingTextStringOnCreature(creature, "Expert en donjons".ColorString(StringUtils.gold));
        LogUtils.LogMessage($"Expert en donjons : dégâts {damage}", LogUtils.LogType.Combat);
      }

      return damage;
    }
  }
}
