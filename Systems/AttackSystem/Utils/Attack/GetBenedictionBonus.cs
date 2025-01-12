using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBenedictionBonus(List<string> noStack)
    {
      int boonBonus = Utils.Roll(4);
      LogUtils.LogMessage($"Bénédiction : +{boonBonus} BA", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.BenedictionEffectTag);

      return boonBonus;
    }
  }
}
