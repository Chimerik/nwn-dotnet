using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int RollWeaponDamage(CNWSCreature creature, NwBaseItem weapon, CNWSCombatAttackData attackData, bool isCriticalRoll = false)
    {
      int numDamageDice = weapon.NumDamageDice 
        + GetFureurOrcBonus(creature) 
        + GetOrcCriticalBonus(creature, attackData, isCriticalRoll)
        + GetEmpaleurCriticalBonus(creature, weapon, isCriticalRoll);

      int damage = HandleWeaponDamageRerolls(creature, weapon, numDamageDice);
      damage = HandleSavageAttacker(creature, weapon, attackData, numDamageDice, damage);

      LogUtils.LogMessage($"{weapon.Name.ToString()} - {numDamageDice}d{weapon.DieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
