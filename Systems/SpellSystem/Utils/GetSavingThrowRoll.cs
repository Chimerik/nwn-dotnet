using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, Ability ability, int saveDC, int advantage, SpellConfig.SavingThrowFeedback feedback, bool fromSpell = false)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, ability)
        + target.GetAbilityModifier(ability)
        + ItemUtils.GetShieldMasterBonusSave(target, ability);

      if(fromSpell && target.ActiveEffects.Any(e => e.Tag == EffectSystem.SensDeLaMagieEffectTag))
        proficiencyBonus += NativeUtils.GetCreatureProficiencyBonus(target);

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
