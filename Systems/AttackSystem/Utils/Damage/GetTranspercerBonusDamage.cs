using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetTranspercerBonusDamage(bool isCritical, List<string> noStack)
    {
      noStack.Add(EffectSystem.TranspercerEffectTag);

      if (isCritical)
        return 0;

      LogUtils.LogMessage("Transpercer : Dégâts +2", LogUtils.LogType.Combat);

      return 2;
    }
  }
}
