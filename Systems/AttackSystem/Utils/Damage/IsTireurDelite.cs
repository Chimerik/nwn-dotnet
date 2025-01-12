using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int  GetTireurDeliteBonusDamage(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem weapon, bool isCritical)
    {
      int bonusDamage = 0;

      if(!isCritical && attackData.m_bRangedAttack.ToBool()
        && attacker.m_pStats.HasFeat(CustomSkill.TireurDelite).ToBool())
      {
        bonusDamage = GetCreatureWeaponProficiencyBonus(attacker, weapon);

        if (bonusDamage > 0)
          LogUtils.LogMessage($"Tireur d'Elite : +{bonusDamage}", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
