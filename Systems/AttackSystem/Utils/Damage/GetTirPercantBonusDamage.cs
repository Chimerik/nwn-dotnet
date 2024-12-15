using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetTirPercantBonusDamage(CNWSCreature creature)
    {
      int bonusDamage = 0;

      if(creature.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.TirPercantEffectExoTag).ToBool()))
      {
        bonusDamage += 2;
        LogUtils.LogMessage("Tir Perçant : Dégâts +2", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
