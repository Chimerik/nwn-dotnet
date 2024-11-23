using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static async void OnAttackFrappeDivine(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature targetCreature)
        return;

      NwCreature caster = onAttack.Attacker;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwItem mainWeapon = caster.GetItemInSlot(InventorySlot.RightHand);
          
          if (mainWeapon is not null && ItemUtils.IsWeapon(mainWeapon.BaseItem))
          { 
            int nbDice = onAttack.Attacker.GetClassInfo(ClassType.Cleric).Level > 13 ? 2 : 1;
            if (onAttack.AttackResult == AttackResult.CriticalHit)
              nbDice *= 2;

            DamageType damageType = caster.GetObjectVariable<PersistentVariableInt>(CreatureUtils.FrappeDivineVariable).HasValue 
              ? (DamageType)caster.GetObjectVariable<PersistentVariableInt>(CreatureUtils.FrappeDivineVariable).Value : DamageType.Divine;

            NWScript.AssignCommand(onAttack.Attacker, () => targetCreature.ApplyEffect(EffectDuration.Instant, 
              Effect.Damage(NwRandom.Roll(Utils.random, 8, nbDice), damageType)));

            EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.FrappeDivineEffectTag);
            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary,
              EffectSystem.Cooldown(onAttack.Attacker, 6, CustomSkill.ClercFrappeDivine), NwTimeSpan.FromRounds(1)));

            await NwTask.NextFrame();
            onAttack.Attacker.OnCreatureAttack -= OnAttackFrappeDivine;
          }

          break;
      }
    }
  }
}
