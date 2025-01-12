using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetRapetissementMalus(bool isCritical, List<string> noStack)
    {
      if (isCritical)
        return 0;

      int roll = Utils.Roll(4);
      LogUtils.LogMessage($"Rapetissement : +{roll}", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.RapetissementEffectTag);

      return roll;
    }
  }
}
