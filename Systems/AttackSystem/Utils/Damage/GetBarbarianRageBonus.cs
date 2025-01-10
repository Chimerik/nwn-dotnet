using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianRageBonus(CNWSCreature creature, CGameEffect eff, List<string> noStack, Anvil.API.Ability attackAbility)
    {
      if (noStack.Contains(EffectSystem.BarbarianRageEffectTag))
        return 0;

      int roll = 0;

      if (attackAbility == Anvil.API.Ability.Strength)
      {
        roll = eff.GetInteger(5);
        LogUtils.LogMessage($"Rage du Barbare : +{roll} dégâts", LogUtils.LogType.Combat);
      }

      noStack.Add(EffectSystem.BarbarianRageEffectTag);

      return roll;  
    }
  }
}
