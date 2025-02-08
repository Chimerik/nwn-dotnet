using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleEntaille(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, CNWSCombatAttackData attackData)
    {
      if (attackData.m_nWeaponAttackType != 2 || !attacker.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.EntailleEffectTag))
        return;

      EffectUtils.RemoveTaggedNativeEffect(attacker, EffectSystem.EntailleEffectTag);
      attacker.m_ScriptVars.SetInt("_ENTAILLE_BONUS_ATTACK".ToExoString(), 1);
      combatRound.AddWhirlwindAttack(currentTarget.m_idSelf, 1);
    }
  }
}
