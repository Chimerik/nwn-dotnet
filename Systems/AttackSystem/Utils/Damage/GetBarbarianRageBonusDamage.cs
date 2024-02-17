using System.Linq;
using Anvil.API;
using NWN.Native.API;
using Feat = NWN.Native.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianRageBonusDamage(CNWSCreature creature, CNWSCombatAttackData data)
    {
      if (data.m_bRangedAttack.ToBool() || !creature.m_pStats.HasFeat((ushort)Feat.BarbarianRage).ToBool()
        || !creature.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.barbarianRageEffectExoTag).ToBool()))
        return 0;

      int barbarianLevel = creature.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Barbarian);

      if (barbarianLevel < 9)
      {
        LogUtils.LogMessage($"Barbare (< 9) - Rage : +2 dégâts", LogUtils.LogType.Combat);
        return 2;
      }
      else if (barbarianLevel < 16)
      {
        LogUtils.LogMessage($"Barbare (< 16) - Rage : +3 dégâts", LogUtils.LogType.Combat);
        return 3;
      }
      else
      {
        LogUtils.LogMessage($"Barbare (max) - Rage : +4 dégâts", LogUtils.LogType.Combat);
        return 4;
      }
    }
  }
}
