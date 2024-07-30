using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackPaumeVibratoire(OnCreatureAttack onAttack)
    {
      if (onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) is null)
      {
        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.AutomaticHit:
          case AttackResult.CriticalHit:

            if (onAttack.Target is NwCreature target)
            {
              SpellConfig.SavingThrowFeedback feedback = new();
              int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Wisdom);
              int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
              int advantage = GetCreatureAbilityAdvantage(target, Ability.Constitution);
              int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Constitution, DC, advantage, feedback);
              bool saveFailed = totalSave < DC;

              StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, $"{onAttack.Attacker.Name.ColorString(ColorConstants.Cyan)} - Paume Vibratoire", StringUtils.gold, true, true);
              SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, DC, totalSave, saveFailed, Ability.Constitution);

              if (saveFailed)
                NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Instant, Effect.Death(true, true)));
              else
                NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Instant
                  , Effect.Damage(NwRandom.Roll(Utils.random, 10, 10), CustomDamageType.Necrotic)));

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackPaumeVibratoire;
            }

            break;
        }
      }
    }
  }
}
