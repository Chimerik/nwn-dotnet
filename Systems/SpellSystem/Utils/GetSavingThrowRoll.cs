using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, SpellEntry spellEntry, int advantage, SpellConfig.SavingThrowFeedback feedback)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, spellEntry) + ItemUtils.GetShieldMasterBonusSave(target, spellEntry.savingThrowAbility);
      int saveRoll = NativeUtils.HandleHalflingLuck(target, NativeUtils.HandleChanceDebordante(target, Utils.RollAdvantage(advantage)));

      feedback.dexProficiencyBonus = proficiencyBonus;
      feedback.saveRoll = saveRoll;

      return saveRoll + proficiencyBonus;
    }
    public static int GetConcentrationSaveRoll(NwCreature target, int advantage, SpellConfig.SavingThrowFeedback feedback)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, Ability.Constitution);
      int saveRoll = NativeUtils.HandleHalflingLuck(target, NativeUtils.HandleChanceDebordante(target, Utils.RollAdvantage(advantage)));

      feedback.dexProficiencyBonus = proficiencyBonus;
      feedback.saveRoll = saveRoll;

      return saveRoll + proficiencyBonus;
    }
  }
}
