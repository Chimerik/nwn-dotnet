using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleSavageAttacker(CNWSCreature creature, NwBaseItem weapon, CNWSCombatAttackData attackData, int numDamageDice, int damage, int dieToRoll)
    {
      if (!attackData.m_bRangedAttack.ToBool() && creature.m_pStats.HasFeat(CustomSkill.AgresseurSauvage).ToBool())
      {
        int secondaryDamageRoll = HandleWeaponDamageRerolls(creature, weapon, numDamageDice, dieToRoll);
        int reroll = damage > secondaryDamageRoll ? damage : secondaryDamageRoll;
        LogUtils.LogMessage($"Agresseur Sauvage reroll {damage} vs {secondaryDamageRoll} = {reroll}", LogUtils.LogType.Combat);
        return reroll;
      }

      return damage;
    }
  }
}
