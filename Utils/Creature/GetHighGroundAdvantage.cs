using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetHighGroundAdvantage(CNWSCreature attacker, CNWSCombatAttackData attackData, CNWSCreature target = null)
    {
      return target is null || attackData.m_bRangedAttack < 1 || attacker.m_vPosition.z < target.m_vPosition.z + 3
        ? target is null || attackData.m_bRangedAttack < 1 || target.m_vPosition.z < attacker.m_vPosition.z + 3 
        ? 0 : -1 
        : 1;
    }
  }
}
