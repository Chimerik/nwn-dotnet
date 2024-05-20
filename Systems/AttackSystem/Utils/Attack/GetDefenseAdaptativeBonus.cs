using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDefenseAdaptativeBonus(CNWSCreature creature, CNWSCreature attacker)
    {
      if (creature.m_pStats.HasFeat(CustomSkill.ChasseurDefenseAdaptative).ToBool()
        && creature.m_appliedEffects.Any(e => e.m_sCustomTag == EffectSystem.DefenseAdaptativeEffectExoTag && e.m_oidCreator == attacker.m_idSelf))
      {
        LogUtils.LogMessage("Défense adaptative : +4 CA", LogUtils.LogType.Combat);
        return 4;
      }

      return 0;
    }
  }
}
