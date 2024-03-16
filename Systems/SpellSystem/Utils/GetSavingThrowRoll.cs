using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, Ability ability, int saveDC, int advantage, SpellConfig.SavingThrowFeedback feedback, bool fromSpell = false)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, ability);

      LogUtils.LogMessage($"JDS proficiency bonus {ability} : {proficiencyBonus}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"JDS modifier {ability} : {target.GetAbilityModifier(ability)}", LogUtils.LogType.Combat);

      proficiencyBonus += target.GetAbilityModifier(ability) + ItemUtils.GetShieldMasterBonusSave(target, ability);

      if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.SensDeLaMagieEffectTag))
        foreach(var eff in target.ActiveEffects)
        {
          switch(eff.Tag) 
          {
            case EffectSystem.SensDeLaMagieEffectTag: if(fromSpell) proficiencyBonus += NativeUtils.GetCreatureProficiencyBonus(target); break;
            case EffectSystem.WildMagicBienfaitEffectTag: proficiencyBonus += NwRandom.Roll(Utils.random, 4); break;
          }
        }

      int saveRoll = Utils.RollAdvantage(advantage);
      saveRoll = NativeUtils.HandleChanceDebordante(target, saveRoll);
      saveRoll = NativeUtils.HandleHalflingLuck(target, saveRoll);

      if(ability == Ability.Strength)
        saveRoll = BarbarianUtils.HandleBarbarianPuissanceIndomptable(target, saveRoll);

      if (saveRoll + proficiencyBonus < saveDC)
        saveRoll = FighterUtils.HandleInflexible(target, saveRoll);

      if (saveRoll + proficiencyBonus < saveDC)
        saveRoll = MonkUtils.HandleDiamondSoul(target, saveRoll);
        
      feedback.proficiencyBonus = proficiencyBonus;
      feedback.saveRoll = saveRoll;

      return saveRoll + proficiencyBonus;
    }
  }
}
