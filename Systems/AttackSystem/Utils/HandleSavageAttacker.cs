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
        return damage > secondaryDamageRoll ? damage : secondaryDamageRoll;
      }

      return damage;
    }
  }
}
