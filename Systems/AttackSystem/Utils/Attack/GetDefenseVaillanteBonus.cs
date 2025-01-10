using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDefenseVaillanteBonus(CNWSCreature creature)
    {
      var eff = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.CompareNoCase(EffectSystem.defenseVaillanteEffectExoTag).ToBool());

      if (eff is not null)
      {
        creature.RemoveEffect(eff);
        LogUtils.LogMessage($"Defense Vaillante : +{eff.m_nCasterLevel} CA", LogUtils.LogType.Combat);
        return eff.m_nCasterLevel;
      }

      return 0;
    }
  }
}
