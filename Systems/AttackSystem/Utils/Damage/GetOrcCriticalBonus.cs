using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetOrcCriticalBonus(CNWSCreature creature, CNWSCombatAttackData attackData, bool isCriticalRoll)
    {
      if (isCriticalRoll && !attackData.m_bRangedAttack.ToBool())
      {
        switch (creature.m_pStats.m_nRace)
        {
          case CustomRace.HalfOrc:
          case CustomRace.HumanoidOrc: return 1;;
        }
      }

      return 0;
    }
  }
}
