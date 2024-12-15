using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackEraflure(OnCreatureAttack onAttack)
    {
      if (onAttack.IsRangedAttack)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Miss:
        case AttackResult.Concealed:
        case AttackResult.MissChance:
        case AttackResult.Parried:

          NwItem weapon;

          switch(onAttack.WeaponAttackType)
          {
            case WeaponAttackType.MainHand:
            case WeaponAttackType.HastedAttack: weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand); break;
            case WeaponAttackType.Offhand: weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand); break;
            default: return;
          }

          if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.HeavyFlail, BaseItemType.Morningstar, BaseItemType.LightMace, 
            BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer)))
          {
            int modifier = onAttack.Attacker.GetAbilityModifier(NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

            if(modifier > 0)
              NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.Damage(modifier, DamageType.Bludgeoning)));
          }

          break;
      } 
    }
  }
}
