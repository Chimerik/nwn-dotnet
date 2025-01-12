using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDegatsBotteSecrete(CNWSCreature creature, CGameEffect eff, List<string> noStack)
    {
      int bonusDamage = eff.GetInteger(5);
      LogUtils.LogMessage($"Dégâts Botte Secrête : +{bonusDamage}", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.BotteSecreteEffectTag);

      return bonusDamage;
    }
  }
}
