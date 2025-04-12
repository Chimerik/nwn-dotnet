using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetStyleArmeDeJetBonusDamage(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSItem weapon, bool isCritical)
    {
      int bonusDamage = 0;

      if(!isCritical && attackData.m_bRangedAttack.ToBool()
        && attacker.m_pStats.HasFeat(CustomSkill.FightingStyleArmeDeJet).ToBool()
        && Utils.In((int)weapon.m_nBaseItem, (int)BaseItem.Sling, (int)BaseItem.Dart, (int)BaseItem.ThrowingAxe))
      {
        bonusDamage = 2;
        LogUtils.LogMessage("Style de Combat - Arme de Jet : +2", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
