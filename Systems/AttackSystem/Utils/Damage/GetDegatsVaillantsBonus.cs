using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDegatsVaillantsBonus(CNWSCreature creature, CGameEffect eff, List<string> noStack)
    {
      var casterLevel = eff.m_nCasterLevel;
      creature.RemoveEffect(eff);
      LogUtils.LogMessage($"Dégâts vaillants : +{casterLevel}", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.DegatsVaillanteEffectTag);

      return casterLevel;
    }
  }
}
