using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static void OnAttackEscrime(OnCreatureAttack onAttack)
    {
      NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsMeleeWeapon(weapon.BaseItem)
        || onAttack.Attacker.ActiveEffects.Any(e => e.Tag == EffectSystem.EscrimeEffectTag))
        return;

      onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Escrime, NwTimeSpan.FromRounds(1));
    }
  }
}
