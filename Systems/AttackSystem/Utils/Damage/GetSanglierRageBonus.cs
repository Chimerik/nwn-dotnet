using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSanglierRageBonus(CNWSCreature creature, CGameEffect eff, List<string> noStack, Anvil.API.Ability attackAbility)
    {
      if (noStack.Contains(EffectSystem.RageDuSanglierEffectTag))
        return 0;

      int roll = 0;

      if (attackAbility == Anvil.API.Ability.Strength)
      {
        roll = eff.GetInteger(5);
        LogUtils.LogMessage($"Rage du SAnglier : +{roll} dégâts", LogUtils.LogType.Combat);
      }

      noStack.Add(EffectSystem.RageDuSanglierEffectTag);

      return roll;  
    }
  }
}
