using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCriticalRange(CNWSCreature creature, CNWSItem weapon, CNWSCombatAttackData data)
    {
      int criticalRange = 20;
      
      if(weapon is not null 
        && !data.m_bRangedAttack.ToBool() 
        && creature.m_pStats.HasFeat(CustomSkill.Broyeur).ToBool() 
        && NwBaseItem.FromItemId((int)weapon.m_nBaseItem).WeaponType.Any(d => d == Anvil.API.DamageType.Bludgeoning)) 
        criticalRange -= 1;

      for (byte i = 0; i < creature.m_pStats.m_nNumMultiClasses; i++)
      {
        CNWSCreatureStats_ClassInfo classInfo = creature.m_pStats.GetClassInfo(i);
        if (classInfo.m_nClass == CustomClass.Champion)
        {
          criticalRange -= classInfo.m_nLevel < 15 ? 1 : 2;
          break;
        }
      }

      return criticalRange;
    }
  }
}
