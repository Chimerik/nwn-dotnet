using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianBrutalCriticalBonus(CNWSCreature creature, bool isRangedAttack, bool isCriticalRoll)
    {
      int barbarianLevel = creature.m_pStats.GetNumLevelsOfClass(CustomClass.Barbarian);

      if (isRangedAttack || !isCriticalRoll || barbarianLevel < 9)
        return 0; 

      if (barbarianLevel < 13)
      {
        LogUtils.LogMessage($"Barbare - Critique brutal : +1 dé de dégâts", LogUtils.LogType.Combat);
        return 1;
      }
      else if (barbarianLevel < 17)
      {
        LogUtils.LogMessage($"Barbare - Critique brutal : +2 dé de dégâts", LogUtils.LogType.Combat);
        return 2;
      }
      else
      {
        LogUtils.LogMessage($"Barbare - Critique brutal : +3 dé de dégâts", LogUtils.LogType.Combat);
        return 3;
      }
    }
  }
}
