using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void SkillsBindings()
        {
          classRow.Children.Clear();
          classRow.Children.Add(classRowSpacer);

          var traitList = targetPlayer.learnableSkills.Values.Where(e => e.category == SkillSystem.Category.StartingTraits);
          var colWidth = windowWidth / 7.5f;

          foreach (var origine in traitList)
          {
            classRow.Children.Add(new NuiColumn() { Width = colWidth, Height = classRow.Height / 1.1f, Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiButtonImage(origine.icon) { Width = windowWidth / 8, Height = windowWidth / 8, Tooltip = origine.name } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiLabel(origine.name) { Width = windowWidth / 8, Height = windowWidth / 30, VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Tooltip = origine.name } } }
            } });

            classRow.Children.Add(classRowSpacer);
          }

          classRowSpacer.Width = ((windowWidth / 1.1f) - (traitList.Count() * colWidth)) / (traitList.Count() + 1);
          classGroup.SetLayout(player.oid, nuiToken.Token, classRow);

          proficiency.SetBindValue(player.oid, nuiToken.Token, $"Bonus de maîtrise (+{NativeUtils.GetCreatureProficiencyBonus(target)})");
          skillTitle.SetBindValue(player.oid, nuiToken.Token, "Description");

          str.SetBindValue(player.oid, nuiToken.Token, $"Force ({(target.GetAbilityModifier(Ability.Strength) < 0 ? "" : "+")}{target.GetAbilityModifier(Ability.Strength)})");
          dex.SetBindValue(player.oid, nuiToken.Token, $"Dextérité ({(target.GetAbilityModifier(Ability.Dexterity) < 0 ? "" : "+")}{target.GetAbilityModifier(Ability.Dexterity)})");
          intel.SetBindValue(player.oid, nuiToken.Token, $"Intelligence ({(target.GetAbilityModifier(Ability.Intelligence) < 0 ? "" : "+")}{target.GetAbilityModifier(Ability.Intelligence)})");
          wis.SetBindValue(player.oid, nuiToken.Token, $"Sagesse ({(target.GetAbilityModifier(Ability.Wisdom) < 0 ? "" : "+")}{target.GetAbilityModifier(Ability.Wisdom)})");
          cha.SetBindValue(player.oid, nuiToken.Token, $"Charisme ({(target.GetAbilityModifier(Ability.Charisma) < 0 ? "" : "+")}{target.GetAbilityModifier(Ability.Charisma)})");

          int skillScore = CreatureUtils.GetSkillScore(target, Ability.Strength, CustomSkill.AthleticsProficiency, true);
          athletism.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Dexterity, CustomSkill.AcrobaticsProficiency, true);
          acrobatie.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Intelligence, CustomSkill.ArcanaProficiency, true);
          arcanes.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Wisdom, CustomSkill.AnimalHandlingProficiency, true);
          dressage.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Charisma, CustomSkill.DeceptionProficiency, true);
          deception.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Intelligence, CustomSkill.HistoryProficiency, true);
          history.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Wisdom, CustomSkill.InsightProficiency, true);
          insight.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Charisma, CustomSkill.IntimidationProficiency, true);
          intimidation.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Intelligence, CustomSkill.InvestigationProficiency, true);
          investigation.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Wisdom, CustomSkill.MedicineProficiency, true);
          medecine.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Intelligence, CustomSkill.NatureProficiency, true);
          nature.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Wisdom, CustomSkill.PerceptionProficiency, true);
          perception.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Charisma, CustomSkill.PerformanceProficiency, true);
          performance.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Charisma, CustomSkill.PersuasionProficiency, true);
          persuasion.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Intelligence, CustomSkill.ReligionProficiency, true);
          religion.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Dexterity, CustomSkill.SleightOfHandProficiency, true);
          sleightOfHand.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Dexterity, CustomSkill.StealthProficiency, true);
          stealth.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");
          skillScore = CreatureUtils.GetSkillScore(target, Ability.Wisdom, CustomSkill.SurvivalProficiency, true);
          survival.SetBindValue(player.oid, nuiToken.Token, $"{(skillScore < 0 ? "" : "+")}{skillScore}");

          athletismProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          acrobatieProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          arcanesProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          dressageProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          deceptionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          historyProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          insightProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          intimidationProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          investigationProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          medecineProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          natureProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          perceptionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          performanceProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          persuasionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          religionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          sleightOfHandProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          stealthProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          survivalProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, true);

          LearnableSkill skillProficiency;

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.AthleticsExpertise, out var expertise) && expertise.currentLevel > 0)
            athletismProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise"); 
          else if(targetPlayer.learnableSkills.TryGetValue(CustomSkill.AthleticsProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            athletismProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            athletismProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.AcrobaticsExpertise, out expertise) && expertise.currentLevel > 0)
            acrobatieProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.AcrobaticsProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            acrobatieProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            acrobatieProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.ArcanaExpertise, out expertise) && expertise.currentLevel > 0)
            arcanesProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.ArcanaProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            arcanesProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            arcanesProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.AnimalHandlingExpertise, out expertise) && expertise.currentLevel > 0)
            dressageProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.AnimalHandlingProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            dressageProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            dressageProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.DeceptionExpertise, out expertise) && expertise.currentLevel > 0)
            deceptionProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.DeceptionProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            deceptionProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            deceptionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.HistoryExpertise, out expertise) && expertise.currentLevel > 0)
            historyProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.HistoryProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            historyProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            historyProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.InsightExpertise, out expertise) && expertise.currentLevel > 0)
            insightProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.InsightProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            insightProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            insightProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.IntimidationExpertise, out expertise) && expertise.currentLevel > 0)
            intimidationProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.IntimidationProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            intimidationProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            intimidationProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.InvestigationExpertise, out expertise) && expertise.currentLevel > 0)
            investigationProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.InvestigationProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            investigationProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            investigationProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.MedicineExpertise, out expertise) && expertise.currentLevel > 0)
            medecineProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.MedicineProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            medecineProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            medecineProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.NatureExpertise, out expertise) && expertise.currentLevel > 0)
            natureProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.NatureProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            natureProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            natureProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.PerceptionExpertise, out expertise) && expertise.currentLevel > 0)
            perceptionProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.PerceptionProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            perceptionProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            perceptionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.PerformanceExpertise, out expertise) && expertise.currentLevel > 0)
            performanceProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.PerformanceProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            performanceProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            performanceProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.PersuasionExpertise, out expertise) && expertise.currentLevel > 0)
            persuasionProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.PersuasionProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            persuasionProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            persuasionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.ReligionExpertise, out expertise) && expertise.currentLevel > 0)
            religionProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.ReligionProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            religionProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            religionProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.SleightOfHandExpertise, out expertise) && expertise.currentLevel > 0)
            sleightOfHandProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.SleightOfHandProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            sleightOfHandProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            sleightOfHandProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.StealthExpertise, out expertise) && expertise.currentLevel > 0)
            stealthProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.StealthProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            stealthProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            stealthProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.SurvivalExpertise, out expertise) && expertise.currentLevel > 0)
            survivalProficiency.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.SurvivalProficiency, out skillProficiency) && skillProficiency.currentLevel > 0)
            survivalProficiency.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            survivalProficiencyVisibility.SetBindValue(player.oid, nuiToken.Token, false);

        }
      }
    }
  }
}
