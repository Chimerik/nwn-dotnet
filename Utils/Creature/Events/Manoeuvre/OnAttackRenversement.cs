using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackRenversement(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;
      
      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Strength) > onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) ? onAttack.Attacker.GetAbilityModifier(Ability.Strength) : onAttack.Attacker.GetAbilityModifier(Ability.Dexterity);
          int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;

          LogUtils.LogMessage($"--- {onAttack.Attacker.Name} renversement contre {target.Name} ---", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Renversement", ColorConstants.Red, true, true);

          if (GetSavingThrow(onAttack.Attacker, target, Ability.Strength, DC, effectType: SpellConfig.SpellEffectType.Knockdown) == SavingThrowResult.Failure)
            EffectSystem.ApplyKnockdown(target, CreatureSize.Large, 1);

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackRenversement;

          break;
      }
    }
  }
}
