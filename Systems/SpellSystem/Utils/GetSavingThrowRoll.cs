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

      foreach(var eff in target.ActiveEffects)
      {
        switch(eff.Tag) 
        {
          case EffectSystem.SensDeLaMagieEffectTag:
            
            if (fromSpell)
            {
              int bonus = NativeUtils.GetCreatureProficiencyBonus(target);
              proficiencyBonus += bonus;
              LogUtils.LogMessage($"Magie Sauvage - Sens de la magie : +{bonus}", LogUtils.LogType.Combat);
            }

            break;
          case EffectSystem.WildMagicBienfaitEffectTag:

            int bienfait = NwRandom.Roll(Utils.random, 4);
            proficiencyBonus += bienfait;
            LogUtils.LogMessage($"Magie Sauvage - Bienfait : +{bienfait}", LogUtils.LogType.Combat);

            break;
        }
      }

      int saveRoll = NativeUtils.HandlePresage(target); // Si présage, alors on remplace totalement le jet de la cible

      if (saveRoll < 1)
      {
        saveRoll = Utils.RollAdvantage(advantage);
        saveRoll = NativeUtils.HandleChanceDebordante(target, saveRoll);
        saveRoll = NativeUtils.HandleHalflingLuck(target, saveRoll);

        if (ability == Ability.Strength)
          saveRoll = BarbarianUtils.HandleBarbarianPuissanceIndomptable(target, saveRoll);

        if (saveRoll + proficiencyBonus < saveDC)
          saveRoll = FighterUtils.HandleInflexible(target, saveRoll);

        if (saveRoll + proficiencyBonus < saveDC)
          saveRoll = MonkUtils.HandleDiamondSoul(target, saveRoll);

        int inspirationBonus = 0;
        Effect inspirationEffect = null;

        foreach (var eff in target.ActiveEffects)
          if (eff.Tag == EffectSystem.InspirationBardiqueEffectTag)
          {
            inspirationBonus = eff.CasterLevel;
            inspirationEffect = eff;
            break;
          }

        if (inspirationBonus > 0 && saveRoll + proficiencyBonus < saveDC && saveRoll + proficiencyBonus + inspirationBonus >= saveDC)
        {
          proficiencyBonus += inspirationBonus;

          LogUtils.LogMessage($"Activation Inspiration Bardique : +{inspirationBonus}", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(target, $"Inspiration Bardique (+{StringUtils.ToWhitecolor(inspirationBonus)})".ColorString(StringUtils.gold), StringUtils.gold, true, true);
          target.RemoveEffect(inspirationEffect);
        }
        else if(inspirationBonus < 0 && saveRoll + proficiencyBonus >= saveDC && saveRoll + proficiencyBonus + inspirationBonus < saveDC)
        {
          proficiencyBonus += inspirationBonus;

          LogUtils.LogMessage($"Activation Mots Cinglants : {inspirationBonus}", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(target, $"Mots Cinglants ({StringUtils.ToWhitecolor(inspirationBonus)})".ColorString(StringUtils.gold), ColorConstants.Red, true, true);
          target.RemoveEffect(inspirationEffect);
        }
      }

      feedback.proficiencyBonus = proficiencyBonus;
      feedback.saveRoll = saveRoll;

      return saveRoll + proficiencyBonus;
    }
  }
}
