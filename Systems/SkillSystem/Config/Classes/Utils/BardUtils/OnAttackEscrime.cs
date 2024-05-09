using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static void OnAttackEscrime(OnCreatureAttack onAttack)
    {
      NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsMeleeWeapon(weapon.BaseItem))
        return;

      onAttack.Attacker.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.MovementSpeedIncrease(15), Effect.Icon(EffectIcon.MovementSpeedIncrease)), NwTimeSpan.FromRounds(1));
    }
  }
}
