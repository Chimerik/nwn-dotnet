using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAgrandissementBonus(List<string> noStack)
    {
      int roll = Utils.Roll(4);
      LogUtils.LogMessage($"Agrandissement : +{roll}", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.EnlargeEffectTag);

      return roll;
    }
  }
}
