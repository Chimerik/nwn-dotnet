using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleMarqueDuChasseurBonusDamage(CNWSCreature creature, CNWSCreature target, CNWSItem weapon)
    {
      if (weapon is not null && target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.MarqueDuChasseurExoTag).ToBool()
        && (e.m_oidCreator == creature.m_idSelf || e.m_oidCreator == creature.m_oidMaster)))
      {
        int bonus = NwRandom.Roll(Utils.random, creature.m_pStats.HasFeat(CustomSkill.RangerPourfendeur).ToBool() ? 10 : 6);
        LogUtils.LogMessage($"Marque du chasseur : +{bonus} dégâts", LogUtils.LogType.Combat);
        return bonus;
      }

      return 0;
    }
  }
}
