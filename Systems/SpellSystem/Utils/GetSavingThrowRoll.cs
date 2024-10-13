﻿using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, Ability ability, int saveDC, int advantage, SpellConfig.SavingThrowFeedback feedback, bool fromSpell = false)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, ability);
      int abilityModifier = target.GetAbilityModifier(ability);

      LogUtils.LogMessage($"JDS modifier {ability} : {abilityModifier}", LogUtils.LogType.Combat);
      
      proficiencyBonus += abilityModifier 
        + ItemUtils.GetShieldMasterBonusSave(target, ability)
        + DruideUtils.GetSanctuaireNaturelBonusSave(target, ability);

      List<string> protectionNoStack = new();

      foreach(var eff in target.ActiveEffects)
      {
        switch(eff.Tag) 
        {
          case EffectSystem.SensDeLaMagieEffectTag:
            
            if (fromSpell && !protectionNoStack.Contains(EffectSystem.SensDeLaMagieEffectTag))
            {
              int bonus = NativeUtils.GetCreatureProficiencyBonus(target);
              proficiencyBonus += bonus;
              protectionNoStack.Add(EffectSystem.SensDeLaMagieEffectTag);
              LogUtils.LogMessage($"Magie Sauvage - Sens de la magie : +{bonus}", LogUtils.LogType.Combat);
            }

            break;

          case EffectSystem.WildMagicBienfaitEffectTag:

            if (protectionNoStack.Contains(EffectSystem.WildMagicBienfaitEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.WildMagicBienfaitEffectTag);
            int bienfait = NwRandom.Roll(Utils.random, 4);
            proficiencyBonus += bienfait;
            LogUtils.LogMessage($"Magie Sauvage - Bienfait : +{bienfait}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.ProtectionEffectTag:

            if (protectionNoStack.Contains(EffectSystem.ProtectionEffectTag) || eff.Creator is not NwCreature protector)
              break;

            protectionNoStack.Add(EffectSystem.ProtectionEffectTag);
            int protection = protector.GetAbilityModifier(Ability.Charisma);
            proficiencyBonus += protection;
            LogUtils.LogMessage($"Paladin - Aura de Protection : +{protection}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.BenedictionEffectTag:

            if (protectionNoStack.Contains(EffectSystem.BenedictionEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.BenedictionEffectTag);

            int beneBonus = NwRandom.Roll(Utils.random, 4);
            proficiencyBonus -= beneBonus;
            LogUtils.LogMessage($"Bénédiction : +{beneBonus}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.FleauEffectTag:

            if (protectionNoStack.Contains(EffectSystem.FleauEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.FleauEffectTag);

            int fleauMalus = NwRandom.Roll(Utils.random, 4);
            proficiencyBonus -= fleauMalus;
            LogUtils.LogMessage($"Fléau : -{fleauMalus}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.LenteurEffectTag:

            if (protectionNoStack.Contains(EffectSystem.LenteurEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.LenteurEffectTag);

            proficiencyBonus -= 2;
            LogUtils.LogMessage("Lenteur : -2", LogUtils.LogType.Combat);

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
