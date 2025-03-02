using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void ReplaceWeaponDamage(DamageData<int> damageData, DamageType newDamageType)
    {
      int newDamage = damageData.GetDamageByType(newDamageType);

      if (newDamage < 0)
        newDamage = 0;

      damageData.SetDamageByType(newDamageType, newDamage + damageData.GetDamageByType(DamageType.BaseWeapon));
      damageData.SetDamageByType(DamageType.BaseWeapon, -1);
    }

    public static void AddWeaponDamage(DamageData<int> damageData, DamageType damageType, int damage)
    {
      int newDamage = damageData.GetDamageByType(damageType);

      if (newDamage < 0)
        newDamage = 0;

      damageData.SetDamageByType(damageType, newDamage + damage);
    }
  }
}
