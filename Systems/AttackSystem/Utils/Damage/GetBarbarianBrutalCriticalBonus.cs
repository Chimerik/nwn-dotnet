using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianBrutalCriticalBonus(CNWSCreature creature, bool isRangedAttack, bool isCriticalRoll)
    {
      int barbarianLevel = creature.m_pStats.GetNumLevelsOfClass(CustomClass.Barbarian);

      if (isRangedAttack || isCriticalRoll || barbarianLevel < 9)
        return 0;

      if (barbarianLevel < 13)
        return 1;
      else if (barbarianLevel < 17)
        return 2;
      else
        return 3;
    }
  }
}
