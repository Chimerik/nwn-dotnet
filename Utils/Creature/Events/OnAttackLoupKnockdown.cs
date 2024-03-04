using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackLoupKnockdown(OnCreatureAttack onAttack)
    {
      if (onAttack.IsRangedAttack || onAttack.Attacker.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value < 1)
        return;

      NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsMeleeWeapon(weapon.BaseItem))
        return;

      switch(onAttack.AttackResult) 
      {
        case AttackResult.Hit:
        case AttackResult.AutomaticHit:
        case AttackResult.CriticalHit:

          if (onAttack.Target is not NwCreature target || target.Size > CreatureSize.Large)
            return;

          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(2));
          onAttack.Attacker.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value -= 1;

          StringUtils.DisplayStringToAllPlayersNearTarget(target, "Lien du Loup", StringUtils.gold, true);

          break;
      }
      
    }
  }
}
