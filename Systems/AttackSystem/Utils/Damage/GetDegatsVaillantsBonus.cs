using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDegatsVaillantsBonus(CNWSCreature creature)
    {
      foreach (var eff in creature.m_appliedEffects)
        if (eff.m_sCustomTag.CompareNoCase(EffectSystem.degatsVaillanteEffectExoTag).ToBool())
        {
          creature.RemoveEffect(eff);
          LogUtils.LogMessage($"Dégâts vaillants : +{eff.m_nCasterLevel}", LogUtils.LogType.Combat);
          return eff.m_nCasterLevel;
        }

      return 0;
    }
  }
}
