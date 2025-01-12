using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void OnAttackPourfendeurDeColosses(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature targetCreature)
        return;

      NwCreature caster = onAttack.Attacker;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (targetCreature.HP < targetCreature.MaxHP)
          {
            NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

            if (weapon is not null && ItemUtils.IsWeapon(weapon.BaseItem.ItemType))
            {
              DamageType damageType = weapon.BaseItem.WeaponType.FirstOrDefault();

              NWScript.AssignCommand(onAttack.Attacker, () => targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Damage(Utils.Roll(8), damageType)));

              NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary,
                EffectSystem.Cooldown(onAttack.Attacker, 6, CustomSkill.ChasseurProie, CustomSpell.PourfendeurDeColosses), NwTimeSpan.FromRounds(1)));

              EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.PourfendeurDeColossesEffectTag);

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackPourfendeurDeColosses;
            }
          }

          break;
      }
    }
  }
}
