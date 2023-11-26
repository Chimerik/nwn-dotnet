using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsCogneurLourd(CNWSCreature attacker, CNWSCombatAttackData attackData)
    {
      CNWSItem attackWeapon = attacker.m_pcCombatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType);

      return attackWeapon is not null 
        && attacker.m_appliedEffects.Any(e => e.m_sCustomTag == EffectSystem.CogneurLourdExoTag)
        && IsGreatWeaponStyle(NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem), attacker)
        && GetCreatureWeaponProficiencyBonus(attacker, attackWeapon) > 1;
    }
  }
}
