using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackFrappeMeurtriere(OnCreatureAttack onAttack)
    {
      NwCreature attacker = onAttack.Attacker;

      switch(onAttack.AttackResult) 
      {
        case AttackResult.Hit:
        case AttackResult.AutomaticHit:
        case AttackResult.CriticalHit:

          if (!attacker.KnowsFeat((Feat)CustomSkill.AssassinAssassinate) 
            || onAttack.Target is not NwCreature target || !attacker.Classes.Any(c => c.Class.ClassType == ClassType.Rogue && c.Level > 16)
            || !NativeUtils.IsAssassinate(attacker))
            return;

          SpellConfig.SavingThrowFeedback feedback = new();
          int DC = 8 + attacker.GetAbilityModifier(Ability.Dexterity) + NativeUtils.GetCreatureProficiencyBonus(attacker);
          int advantage = GetCreatureAbilityAdvantage(target, Ability.Constitution);
          int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Constitution, DC, advantage, feedback);
          bool saveFailed = totalSave < DC;

          if (saveFailed)
          {
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComBloodSparkMedium));
            attacker.GetObjectVariable<LocalVariableInt>(FrappeMeurtriereVariable).Value = 1;
          }

          StringUtils.DisplayStringToAllPlayersNearTarget(target, "Frappe Meurtrière", StringUtils.gold, true, true);
          LogUtils.LogMessage($"{attacker.Name} - Frappe Meurtrière sur {target.Name}", LogUtils.LogType.Combat);
          SpellUtils.SendSavingThrowFeedbackMessage(attacker, target, feedback, advantage, DC, totalSave, saveFailed, Ability.Constitution);

          break;
      }
      
    }
  }
}
