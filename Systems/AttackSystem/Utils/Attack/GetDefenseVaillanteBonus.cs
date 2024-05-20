using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDefenseVaillanteBonus(CNWSCreature creature)
    {
      foreach (var eff in creature.m_appliedEffects)
        if (eff.m_sCustomTag.CompareNoCase(EffectSystem.defenseVaillanteEffectExoTag).ToBool())
        {
          creature.RemoveEffect(eff);
          LogUtils.LogMessage($"Defense Vaillante : +{eff.m_nCasterLevel} CA", LogUtils.LogType.Combat);
          return eff.m_nCasterLevel;
        }

      return 0;
    }
  }
}
