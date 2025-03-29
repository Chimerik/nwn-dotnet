using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetRayonAffaiblissantDamageReduction(Ability attackAbility, List<string> noStack)
    {
      int roll = 0;
      noStack.Add(EffectSystem.RayonAffaiblissantEffectTag);

      if (attackAbility == Ability.Strength)
      {
        roll = NwRandom.Roll(Utils.random, 8);
        LogUtils.LogMessage($"Rayon Affaiblissant - Réduction de dégâts : {roll} (1d8)", LogUtils.LogType.Combat);
      }
      return roll;
    }
  }
}
