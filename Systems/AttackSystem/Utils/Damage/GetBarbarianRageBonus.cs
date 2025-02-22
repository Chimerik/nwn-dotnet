using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianRageBonus(CNWSCreature creature, CGameEffect eff, List<string> noStack, Anvil.API.Ability attackAbility, bool isCritical)
    {
      if (isCritical)
        return 0;

      int roll = 0;

      if (attackAbility == Anvil.API.Ability.Strength)
      {
        int level = creature.m_pStats.GetNumLevelsOfClass(CustomClass.Barbarian);
        roll = level > 15 ? 4 : level > 8 ? 3 : 2;
        LogUtils.LogMessage($"Rage du Barbare : +{roll} dégâts", LogUtils.LogType.Combat);
      }

      noStack.Add(EffectSystem.BarbarianRageEffectTag);

      return roll;  
    }
  }
}
