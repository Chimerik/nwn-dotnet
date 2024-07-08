using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetSavingThrow(NwCreature attacker, NwCreature target, Ability ability, int saveDC, SpellEntry spellEntry = null)
    {
      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = GetCreatureAbilityAdvantage(target, ability, spellEntry);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, ability, saveDC, advantage, feedback);
      bool saveFailed = totalSave < saveDC;

      SpellUtils.SendSavingThrowFeedbackMessage(attacker, target, feedback, advantage, saveDC, totalSave, saveFailed, ability);

      return saveFailed;
    }
  }
}
