using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFleauMalus(List<string> noStack)
    {
      int fleauMalus = Utils.Roll(4);
      LogUtils.LogMessage($"Fléau : -{fleauMalus} BA", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.FleauEffectTag);

      return fleauMalus;
    }
  }
}
