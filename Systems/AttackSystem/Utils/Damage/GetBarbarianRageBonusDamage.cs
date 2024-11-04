using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianRageBonusDamage(CNWSCreature creature, Anvil.API.Ability attackAbility, bool isMeleeAttack = true)
    {
      if (isMeleeAttack && attackAbility == Anvil.API.Ability.Strength)
      {
        if(creature.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.barbarianRageEffectExoTag).ToBool()))
        {
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
        else if(creature.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.rageDuSanglierEffectExoTag).ToBool()))
        {
          int rangerLevel = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(creature.m_oidMaster).m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Ranger);

          if (rangerLevel < 9)
          {
            LogUtils.LogMessage($"Belluaire (< 9) - Rage : +2 dégâts", LogUtils.LogType.Combat);
            return 2;
          }
          else if (rangerLevel < 16)
          {
            LogUtils.LogMessage($"Belluaire (< 16) - Rage : +3 dégâts", LogUtils.LogType.Combat);
            return 3;
          }
          else
          {
            LogUtils.LogMessage($"Belluaire (max) - Rage : +4 dégâts", LogUtils.LogType.Combat);
            return 4;
          }
        }
      }

      return 0;  
    }
  }
}
