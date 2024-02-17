using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NWN.Core;
using NativeUtils = NWN.Systems.NativeUtils;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackRenversement(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          onAttack.Attacker.OnCreatureAttack -= OnAttackRenversement;

          if (target.Size > onAttack.Attacker.Size + 1)
            onAttack.Attacker?.LoginPlayer.SendServerMessage("Impossible de renverser une créature de cette taille !", ColorConstants.Red);
          else
          {
            SpellConfig.SavingThrowFeedback feedback = new();
            int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Strength) > onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) ? onAttack.Attacker.GetAbilityModifier(Ability.Strength) : onAttack.Attacker.GetAbilityModifier(Ability.Dexterity);
            int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
            int advantage = GetCreatureAbilityAdvantage(target, Ability.Strength);
            int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Strength, tirDC, advantage, feedback);
            bool saveFailed = totalSave < tirDC;

            StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Renversement", ColorConstants.Red, true);
            SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Strength);

            if (saveFailed)
              target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(2));
          }

          break;
      }
    }
  }
}
