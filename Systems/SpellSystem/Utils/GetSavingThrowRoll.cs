using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, SpellEntry spellEntry, int advantage, SpellConfig.SavingThrowFeedback feedback)
    {
      int dexProficiencyBonus = GetSavingThrowProficiencyBonus(target, spellEntry);
      int saveRoll = Utils.RollAdvantage(advantage);

      feedback.dexProficiencyBonus = dexProficiencyBonus;
      feedback.saveRoll = saveRoll;

      return saveRoll + dexProficiencyBonus;
    }
  }
}
