using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetEmpaleurCriticalBonus(CNWSCreature creature, NwBaseItem weapon, bool isCriticalRoll)
    {
      return isCriticalRoll
        && creature.m_pStats.HasFeat(CustomSkill.Empaleur).ToBool()
        && weapon.WeaponType.Any(d => d == Anvil.API.DamageType.Piercing)
        ? 1
        : 0;
    }
  }
}
