using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NWN.Core;
using NativeUtils = NWN.Systems.NativeUtils;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackMenacante(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Attaque Menaçante", ColorConstants.Red, true);

          if (!EffectSystem.IsFrightImmune(target, onAttack.Attacker))
          {
            int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Strength) > onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) ? onAttack.Attacker.GetAbilityModifier(Ability.Strength) : onAttack.Attacker.GetAbilityModifier(Ability.Dexterity);
            int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;

            if(GetSavingThrow(onAttack.Attacker, target, Ability.Wisdom, DC) == SavingThrowResult.Failure)
            {
              NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary,
              EffectSystem.Effroi, NwTimeSpan.FromRounds(1)));
            }
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackMenacante;

          break;
      }
    }
  }
}
