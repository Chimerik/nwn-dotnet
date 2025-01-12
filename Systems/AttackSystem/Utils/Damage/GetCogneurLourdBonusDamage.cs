using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCogneurLourdBonusDamage(CNWSCreature attacker, CNWSItem weapon, bool isCritical)
    {
      int bonusDamage = 0;

      if (!isCritical && attacker.m_pStats.HasFeat(CustomSkill.CogneurLourd).ToBool()
        && IsGreatWeaponStyle(NwBaseItem.FromItemId((int)weapon.m_nBaseItem), attacker))
      {
        bonusDamage = GetCreatureWeaponProficiencyBonus(attacker, weapon);

        if(bonusDamage > 0)
          LogUtils.LogMessage($"Cogneur Lourd : +{bonusDamage}", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
