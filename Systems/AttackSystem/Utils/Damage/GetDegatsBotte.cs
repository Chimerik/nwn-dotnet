using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDegatsBotteSecrete(CNWSCreature creature, CGameEffect eff, bool isCritical)
    {
      int bonusDamage = eff.GetInteger(5);
      bonusDamage *= isCritical ? 2 : 1;
      LogUtils.LogMessage($"Dégâts Botte Secrête : +{bonusDamage}", LogUtils.LogType.Combat);
      return bonusDamage;
    }
  }
}
