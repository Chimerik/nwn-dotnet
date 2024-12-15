using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleEntaille(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound)
    {
      if (!attacker.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.EntailleEffectExoTag).ToBool()))
        return;

      EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.EntailleEffectExoTag);  
      combatRound.AddWhirlwindAttack(currentTarget.m_idSelf, 1);
    }
  }
}
