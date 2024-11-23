using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandlePourfendeurDeColosse(CNWSCreature attacker, CNWSCreature target, CNWSItem weapon)
    {
      if (weapon is null || target.GetMaxHitPoints(1) <= target.GetCurrentHitPoints(0)
        || !attacker.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.PourfendeurDeColossesEffectExoTag).ToBool()))
        return 0;

      EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.PourfendeurDeColossesEffectExoTag);
      int bonusDamage = NwRandom.Roll(Utils.random, 8);
      LogUtils.LogMessage($"Pourfendeur de Colosses : +{bonusDamage}", LogUtils.LogType.Combat);

      return bonusDamage;
    }
  }
}
