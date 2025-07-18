﻿using System;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetSkillScore(NwCreature creature, Ability ability, int skill, bool noLogs = false)
    {
      int score = 0;

      if (skill > -1)
      {
        if (creature.IsLoginPlayerCharacter && PlayerSystem.Players.TryGetValue(creature, out PlayerSystem.Player player))
        {
          if (player.learnableSkills.TryGetValue(skill, out LearnableSkill learnable) && learnable.currentLevel > 0)
          {
            score += NativeUtils.GetCreatureProficiencyBonus(creature);
            if (!noLogs) LogUtils.LogMessage($"Bonus de maîtrise : {score}", LogUtils.LogType.Combat);

            if (player.learnableSkills.TryGetValue(skill + 1, out LearnableSkill expertise) && expertise.currentLevel > 0)
            {
              score *= 2;
              if (!noLogs) LogUtils.LogMessage($"Expertise : {score}", LogUtils.LogType.Combat);
            }
          }
          else
          {

            string tag = ability switch
            {
              Ability.Dexterity => EffectSystem.SavoirAncestralDexteriteEffectTag,
              Ability.Constitution => EffectSystem.SavoirAncestralConstitutionEffectTag,
              Ability.Intelligence => EffectSystem.SavoirAncestralIntelligenceEffectTag,
              Ability.Charisma => EffectSystem.SavoirAncestralCharismeEffectTag,
              Ability.Wisdom => EffectSystem.SavoirAncestralSagesseEffectTag,
              _ => "Invalid"
            };

            if (creature.ActiveEffects.Any(e => e.Tag == tag))
            {
              score += NativeUtils.GetCreatureProficiencyBonus(creature);
              if (!noLogs) LogUtils.LogMessage($"Savoir ancestral - Bonus de maîtrise : {score}", LogUtils.LogType.Combat);
              return score;
            }

            if (creature.KnowsFeat((Feat)CustomSkill.ToucheATout))
            {
              score = (int)Math.Round((double)(NativeUtils.GetCreatureProficiencyBonus(creature) / 2), MidpointRounding.ToZero);
              if (!noLogs) LogUtils.LogMessage($"Touche à tout : {score}", LogUtils.LogType.Combat);

              return score;
            }

            switch (ability)
            {
              case Ability.Strength:
              case Ability.Dexterity:
              case Ability.Constitution:

                if (player.learnableSkills.TryGetValue(CustomSkill.FighterChampion, out LearnableSkill champion) && champion.currentLevel > 6)
                {
                  score = (int)Math.Round((double)(NativeUtils.GetCreatureProficiencyBonus(creature) / 2), MidpointRounding.AwayFromZero);
                  if (!noLogs) LogUtils.LogMessage($"Champion : {score}", LogUtils.LogType.Combat);

                  return score;
                }

                break;
            }
          }
        }
        else
        {
          score += NativeUtils.GetCreatureProficiencyBonus(creature);
          if (!noLogs) LogUtils.LogMessage($"Bonus de maîtrise : {score}", LogUtils.LogType.Combat);
        }
      }

      score += creature.GetAbilityModifier(ability);
      if (!noLogs) LogUtils.LogMessage($"Modificateur de caractéristique : {creature.GetAbilityModifier(ability)}", LogUtils.LogType.Combat);
      score += GetSkillEffectBonus(creature, skill);

      switch(skill)
      {
        case CustomSkill.ArcanaProficiency:
        case CustomSkill.NatureProficiency:

          if (creature.KnowsFeat((Feat)CustomSkill.DruideSage))
          {
            int druideSageBonus = creature.GetAbilityModifier(Ability.Wisdom) > 0 ? creature.GetAbilityModifier(Ability.Wisdom) : 1;
            if (!noLogs) LogUtils.LogMessage($"Druide Sage : +{druideSageBonus}", LogUtils.LogType.Combat);
            score += druideSageBonus;
          }

          break;
      }

      if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.WildMagicBienfaitEffectTag))
      {
        int bienfait = NwRandom.Roll(Utils.random, 4);
        if (!noLogs) LogUtils.LogMessage($"Magie Sauvage - Bienfait : {bienfait}", LogUtils.LogType.Combat);
        score += bienfait;
      }

      if (!noLogs) LogUtils.LogMessage($"Bonus total : {score}", LogUtils.LogType.Combat);

      return score;
    }
  }
}
