using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static void OnAttackFrappesRenforcees(OnCreatureDamage onDamage)
    {
      if (onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon) < 0 || onDamage.DamagedBy is not NwCreature damager 
        || damager.GetItemInSlot(InventorySlot.RightHand) is not null)
        return;

      int damage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);

      //onDamage.Target.ActiveEffects.Any(e => e.EffectType == EffectType.DamageImmunityIncrease)
    }
  }
}
