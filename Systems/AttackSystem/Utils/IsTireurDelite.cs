using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsTireurDelite(CNWSCreature attacker, CNWSCombatAttackData attackData)
    {
      CNWSItem attackWeapon = attacker.m_pcCombatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType);

      return attackWeapon is not null && attackData.m_bRangedAttack.ToBool()
        && attacker.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.TireurDeliteExoTag).ToBool())
        && GetCreatureWeaponProficiencyBonus(attacker, attackWeapon) > 1;
    }
  }
}
