using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleSavageAttacker(CNWSCreature creature, NwBaseItem weapon, CNWSCombatAttackData attackData, int numDamageDice, int damage)
    {
      if (!attackData.m_bRangedAttack.ToBool() && creature.m_pStats.HasFeat(CustomSkill.AgresseurSauvage).ToBool())
      {
        int secondaryDamageRoll = HandleWeaponDamageRerolls(creature, weapon, numDamageDice);
        return damage > secondaryDamageRoll ? damage : secondaryDamageRoll;
      }

      return damage;
    }
  }
}
