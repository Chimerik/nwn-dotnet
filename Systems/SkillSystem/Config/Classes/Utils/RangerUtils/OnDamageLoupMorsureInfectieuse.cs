using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnDamageLoupMorsureInfectieuse(OnCreatureDamage onDamage)
    {
      onDamage.DamageData.SetDamageByType(CustomDamageType.Necrotic, onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon) + onDamage.DamageData.GetDamageByType(CustomDamageType.Necrotic));
      onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, -1);

      NWScript.AssignCommand(onDamage.DamagedBy, () => onDamage.Target.ApplyEffect(EffectDuration.Temporary, EffectSystem.MorsureInfectieuse, NwTimeSpan.FromRounds(1)));
    }
  }
}
