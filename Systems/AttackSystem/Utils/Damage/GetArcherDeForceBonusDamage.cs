using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetArcherDeForceBonusDamage(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem weapon, bool isCritical)
    {
      int bonusDamage = 0;

      if(!isCritical && attackData.m_bRangedAttack.ToBool()
        && attacker.m_pStats.HasFeat(CustomSkill.FightingStyleArcherDeForce).ToBool()
        && Utils.In((int)weapon.m_nBaseItem, (int)BaseItem.Shortbow, (int)BaseItem.Longbow))
      {
        bonusDamage = 1;
        LogUtils.LogMessage("Style de Combat - Archer de Force : +1", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
