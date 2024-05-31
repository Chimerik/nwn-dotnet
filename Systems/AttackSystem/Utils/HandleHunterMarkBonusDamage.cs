using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleHunterMarkBonusDamage(CNWSCreature creature, CNWSCreature target)
    {
      if (target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.hunterMarkEffectExoTag).ToBool()
        && (e.m_oidCreator == creature.m_idSelf || e.m_oidCreator == creature.m_oidMaster)))
      {
        int bonus = NwRandom.Roll(Utils.random, 6);
        LogUtils.LogMessage($"Marque du chasseur : +{bonus} dégâts", LogUtils.LogType.Combat);
        return bonus;
      }

      return 0;
    }
  }
}
