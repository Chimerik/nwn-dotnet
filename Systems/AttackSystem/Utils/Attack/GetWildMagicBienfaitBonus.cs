using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetWildMagicBienfaitBonus(List<string> noStack)
    {
      int boonBonus = Utils.Roll(4);
      LogUtils.LogMessage($"Magie Sauvage : +{boonBonus} BA", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.WildMagicBienfaitEffectTag);

      return boonBonus;
    }
  }
}
