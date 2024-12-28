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

          Ability DCAbility = onAttack.Attacker.GetAbilityModifier(Ability.Strength) > onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) ? Ability.Strength :Ability.Dexterity;

          LogUtils.LogMessage($"--- {onAttack.Attacker.Name} renversement contre {target.Name} ---", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Renversement", ColorConstants.Red, true, true);

          EffectSystem.ApplyKnockdown(target, onAttack.Attacker, DCAbility, Ability.Strength, EffectSystem.Destabilisation);

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackRenversement;

          break;
      }
    }
  }
}
