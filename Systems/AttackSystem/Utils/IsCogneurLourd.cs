using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsCogneurLourd(CNWSCreature attacker, CNWSItem weapon)
    {
      return attacker.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.CogneurLourdExoTag).ToBool())
        && IsGreatWeaponStyle(NwBaseItem.FromItemId((int)weapon.m_nBaseItem), attacker)
        && GetCreatureWeaponProficiencyBonus(attacker, weapon) > 1;
    }
  }
}
