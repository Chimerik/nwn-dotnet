using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetTirPercantBonusDamage(bool isCritical, List<string> noStack)
    {
      noStack.Add(EffectSystem.TirPercantEffectTag);

      if (isCritical)
        return 0;

      LogUtils.LogMessage("Tir Perçant : Dégâts +2", LogUtils.LogType.Combat);
      return 2;
    }
  }
}
