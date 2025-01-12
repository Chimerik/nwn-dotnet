using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetProtectionContreLesLamesMalus(List<string> noStack)
    {
      int fleauMalus = Utils.Roll(4);
      LogUtils.LogMessage($"Protection contre les lames : -{fleauMalus} BA", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.ProtectionContreLesLamesEffectTag);

      return fleauMalus;
    }
  }
}
