using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static async void OnAttackBuveuseDeVie(OnCreatureAttack onAttack)
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
          NwItem secondaryWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

          if ((mainWeapon is not null && mainWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value == caster)
            || (secondaryWeapon is not null && secondaryWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value == caster))
          {
            int conMod = onAttack.Attacker.GetAbilityModifier(Ability.Constitution) > 1 ? onAttack.Attacker.GetAbilityModifier(Ability.Constitution) : 1;

            DamageType damageType = caster.GetObjectVariable<PersistentVariableInt>(CreatureUtils.BuveuseDeVieVariable).HasValue 
              ? (DamageType)caster.GetObjectVariable<PersistentVariableInt>(CreatureUtils.BuveuseDeVieVariable).Value : CustomDamageType.Psychic;

            NWScript.AssignCommand(onAttack.Attacker, () => targetCreature.ApplyEffect(EffectDuration.Instant, 
              Effect.Damage(NwRandom.Roll(Utils.random, 6, 1 + (onAttack.AttackResult == AttackResult.CriticalHit).ToInt()), damageType)));

            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Attacker.ApplyEffect(EffectDuration.Instant,
              Effect.Heal(NwRandom.Roll(Utils.random, 6) + conMod)));
            
            EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.BuveuseDeVieEffectTag);
            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary,
              EffectSystem.Cooldown(onAttack.Attacker, 6, CustomSkill.BuveuseDeVie), NwTimeSpan.FromRounds(1)));

            await NwTask.NextFrame();
            onAttack.Attacker.OnCreatureAttack -= OnAttackBuveuseDeVie;
          }

          break;
      }
    }
  }
}
