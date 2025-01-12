using System.Linq;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleEntaille(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound)
    {
      if (!attacker.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.EntailleEffectTag))
        return;

      EffectUtils.RemoveTaggedNativeEffect(attacker, EffectSystem.EntailleEffectTag);  
      combatRound.AddWhirlwindAttack(currentTarget.m_idSelf, 1);
    }
  }
}
