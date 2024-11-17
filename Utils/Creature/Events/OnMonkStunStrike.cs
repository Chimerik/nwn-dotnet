using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnMonkStunStrike(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target || onAttack.IsRangedAttack)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Wisdom);
          int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Frappe étourdissante", StringUtils.gold, true, true);

          if (GetSavingThrow(onAttack.Attacker, target, Ability.Constitution, DC) == SavingThrowResult.Failure)
            target.ApplyEffect(EffectDuration.Temporary, Effect.Stunned(), NwTimeSpan.FromRounds(1));
          else
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.FrappeEtourdissante, NwTimeSpan.FromRounds(1));

          onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, CustomSkill.MonkStunStrike, 6), NwTimeSpan.FromRounds(1));

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnMonkStunStrike;

          break;
      }
    }
  }
}
