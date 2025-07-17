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
              int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Wisdom);
              int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;

              StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, $"{onAttack.Attacker.Name.ColorString(ColorConstants.Cyan)} - Paume Vibratoire", StringUtils.gold, true, true);

              if (GetSavingThrowResult(target, Ability.Constitution, onAttack.Target, DC) == SavingThrowResult.Failure)
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
