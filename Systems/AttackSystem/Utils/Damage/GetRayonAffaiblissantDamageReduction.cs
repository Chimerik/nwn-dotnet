using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetRayonAffaiblissantDamageReduction(List<string> noStack)
    {
      noStack.Add(EffectSystem.RayonAffaiblissantEffectTag);

      int roll = NwRandom.Roll(Utils.random, 8);
      LogUtils.LogMessage($"Rayon Affaiblissant - Réduction de dégâts : {roll} (1d8)", LogUtils.LogType.Combat);

      return roll;
    }
  }
}
