using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, Ability ability, int saveDC, int advantage, SpellConfig.SavingThrowFeedback feedback)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, ability) + ItemUtils.GetShieldMasterBonusSave(target, ability);
      int saveRoll = Utils.RollAdvantage(advantage);
      saveRoll = NativeUtils.HandleChanceDebordante(target, saveRoll);
      saveRoll = NativeUtils.HandleHalflingLuck(target, saveRoll);

      if(ability == Ability.Strength)
        saveRoll = BarbarianUtils.HandleBarbarianPuissanceIndomptable(target, saveRoll);

      if (saveRoll < saveDC)
        saveRoll = FighterUtils.HandleInflexible(target, saveRoll);

      feedback.proficiencyBonus = proficiencyBonus;
      feedback.saveRoll = saveRoll;

      return saveRoll + proficiencyBonus;
    }
  }
}
