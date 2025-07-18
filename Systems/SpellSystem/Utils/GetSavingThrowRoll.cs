﻿using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, Ability ability, int saveDC, int advantage, SpellConfig.SavingThrowFeedback feedback, SpellEntry spellEntry = null, SpellConfig.SpellEffectType effectType = SpellConfig.SpellEffectType.Invalid)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, ability);
      LogUtils.LogMessage($"JDS {ability} proficiency bonus : {proficiencyBonus}", LogUtils.LogType.Combat);

      int abilityModifier = target.GetAbilityModifier(ability);
      LogUtils.LogMessage($"JDS modifier {ability} : {abilityModifier}", LogUtils.LogType.Combat);

      proficiencyBonus += abilityModifier
        + ItemUtils.GetShieldMasterBonusSave(target, ability);

      List<string> protectionNoStack = new();

      foreach(var eff in target.ActiveEffects)
      {
        switch(eff.Tag) 
        {
          case EffectSystem.LienDeGardeBonusEffectTag:

            if (!protectionNoStack.Contains(EffectSystem.LienDeGardeBonusEffectTag))
            {
              proficiencyBonus += 1;
              protectionNoStack.Add(EffectSystem.LienDeGardeBonusEffectTag);
              LogUtils.LogMessage("Lien de Garde : JDS +1", LogUtils.LogType.Combat);
            }

            break;

          case EffectSystem.SensDeLaMagieEffectTag:

            if (spellEntry is not null && !protectionNoStack.Contains(EffectSystem.SensDeLaMagieEffectTag))
            {
              int bonus = NativeUtils.GetCreatureProficiencyBonus(target);
              proficiencyBonus += bonus;
              protectionNoStack.Add(EffectSystem.SensDeLaMagieEffectTag);
              LogUtils.LogMessage($"Magie Sauvage - Sens de la magie : JDS +{bonus}", LogUtils.LogType.Combat);
            }

            break;

          case EffectSystem.WildMagicBienfaitEffectTag:

            if (protectionNoStack.Contains(EffectSystem.WildMagicBienfaitEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.WildMagicBienfaitEffectTag);
            int bienfait = Utils.Roll(4);
            proficiencyBonus += bienfait;
            LogUtils.LogMessage($"Magie Sauvage - Bienfait : JDS +{bienfait}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.ProtectionEffectTag:

            if (protectionNoStack.Contains(EffectSystem.ProtectionEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.ProtectionEffectTag);
            proficiencyBonus += eff.CasterLevel;
            LogUtils.LogMessage($"Paladin - Aura de Protection : JDS +{eff.CasterLevel}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.BenedictionEffectTag:

            if (protectionNoStack.Contains(EffectSystem.BenedictionEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.BenedictionEffectTag);

            int beneBonus = Utils.Roll(4);
            proficiencyBonus += beneBonus;
            LogUtils.LogMessage($"Bénédiction : JDS +{beneBonus}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.FleauEffectTag:

            if (protectionNoStack.Contains(EffectSystem.FleauEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.FleauEffectTag);

            int fleauMalus = Utils.Roll(4);
            proficiencyBonus -= fleauMalus;
            LogUtils.LogMessage($"Fléau : JDS -{fleauMalus}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.LenteurEffectTag:

            if (protectionNoStack.Contains(EffectSystem.LenteurEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.LenteurEffectTag);

            proficiencyBonus -= 2;
            LogUtils.LogMessage("Lenteur : JDS -2", LogUtils.LogType.Combat);

            break;

          case EffectSystem.FractureMentaleEffectTag:

            if (protectionNoStack.Contains(EffectSystem.FractureMentaleEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.FractureMentaleEffectTag);
            int fractureRoll = Utils.Roll(4);
            proficiencyBonus -= fractureRoll;
            LogUtils.LogMessage($"Fracture Mentale : JDS -{fractureRoll}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.SanctuaireNaturelEffectTag:

            if (ability != Ability.Dexterity || protectionNoStack.Contains(EffectSystem.SanctuaireNaturelEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.SanctuaireNaturelEffectTag);
            proficiencyBonus += 2;
            LogUtils.LogMessage($"Sanctuaire Naturel : JDS +2", LogUtils.LogType.Combat);

            break;

          case EffectSystem.FaveurDuMalinEffectTag:

            if (eff.IntParams[5] != CustomSpell.FaveurDuMalinJDS || protectionNoStack.Contains(EffectSystem.FaveurDuMalinEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.FaveurDuMalinEffectTag);
            int faveurDuMalinRoll = Utils.Roll(10);
            proficiencyBonus += faveurDuMalinRoll;
            LogUtils.LogMessage($"Faveur du Malin : JDS +{faveurDuMalinRoll}", LogUtils.LogType.Combat);

            break;

          case EffectSystem.PolymorphEffectTag:

            int wisMod = target.GetAbilityModifier(Ability.Wisdom);

            if (ability != Ability.Constitution || wisMod < 1 || !target.KnowsFeat((Feat)CustomSkill.DruideResilienceSauvage)
              || protectionNoStack.Contains(EffectSystem.PolymorphEffectTag))
              break;

            protectionNoStack.Add(EffectSystem.PolymorphEffectTag);
            proficiencyBonus += wisMod;
            LogUtils.LogMessage($"Résilience Sauvage : JDS +{wisMod}", LogUtils.LogType.Combat);

            break;
        }
      }

      if (effectType == SpellConfig.SpellEffectType.Death && target.KnowsFeat((Feat)CustomSkill.Survivant3))
      {
        int wisMod = CreatureUtils.GetAbilityModifierMin1(target, Ability.Wisdom);
        proficiencyBonus += wisMod;
        LogUtils.LogMessage($"Survivant III : JDS contre la mort +{wisMod}", LogUtils.LogType.Combat);
      }

      int saveRoll = saveDC > 0 ? NativeUtils.HandlePresage(target) : 0; // Si présage, alors on remplace totalement le jet de la cible

      if (saveRoll < 1)
      {
        saveRoll = Utils.RollAdvantage(advantage);
        saveRoll = NativeUtils.HandleChanceDebordante(target, saveRoll);
        saveRoll = NativeUtils.HandleHalflingLuck(target, saveRoll);

        if (ability == Ability.Strength)
          saveRoll = BarbarianUtils.HandleBarbarianPuissanceIndomptable(target, saveRoll);

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
        else if(saveDC > 0 && inspirationBonus < 0 && saveRoll + proficiencyBonus >= saveDC && saveRoll + proficiencyBonus + inspirationBonus < saveDC)
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
