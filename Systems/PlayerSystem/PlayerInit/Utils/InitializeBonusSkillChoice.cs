using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public async void InitializeBonusSkillChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").HasValue)
        {
          int source = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value;
          int option = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Value;

          List<NuiComboEntry> skillList = new();

          switch (source)
          {
            case CustomSkill.FighterArcaneArcher:

              switch ((SkillConfig.SkillOptionType)option)
              {
                case SkillConfig.SkillOptionType.Proficiency: 

                  if (!learnableSkills.TryGetValue(CustomSkill.ArcanaProficiency, out LearnableSkill arcana) || arcana.currentLevel < 1)
                    skillList.Add(new NuiComboEntry("Arcane - Maîtrise", CustomSkill.ArcanaProficiency));

                  if (!learnableSkills.TryGetValue(CustomSkill.NatureProficiency, out LearnableSkill nat) || nat.currentLevel < 1)
                    skillList.Add(new NuiComboEntry("Nature - Maîtrise", CustomSkill.NatureProficiency));

                  break;

                case SkillConfig.SkillOptionType.Cantrip:

                  if (!learnableSkills.TryGetValue(CustomSkill.ArcaneArcherPrestidigitation, out LearnableSkill prestidigitation) || prestidigitation.currentLevel < 1)
                    skillList.Add(new NuiComboEntry("Prestidigitation - Tour de magie", CustomSkill.ArcaneArcherPrestidigitation));

                  if (!learnableSkills.TryGetValue(CustomSkill.ArcaneArcherDruidisme, out LearnableSkill druidisme) || druidisme.currentLevel < 1)
                    skillList.Add(new NuiComboEntry("Druidisme - Tour de magie", CustomSkill.ArcaneArcherDruidisme));

                  break;
              }

              break;

            case CustomRace.Drow:
            case CustomRace.HighElf:
            case CustomRace.WoodElf:

              if (!learnableSkills.TryGetValue(CustomSkill.PerceptionProficiency, out LearnableSkill perception) || perception.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Perception - Maîtrise", CustomSkill.PerceptionProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.SurvivalProficiency, out LearnableSkill survie) || survie.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Survie - Maîtrise", CustomSkill.SurvivalProficiency));

              break;

            case CustomRace.DrowHalfElf:
            case CustomRace.HighHalfElf:
            case CustomRace.WoodHalfElf:
            case CustomRace.Human:

              if (!learnableSkills.TryGetValue(CustomSkill.AcrobaticsProficiency, out LearnableSkill acrobatie) || acrobatie.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Acrobatie - Maîtrise", CustomSkill.AcrobaticsProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.AnimalHandlingProficiency, out LearnableSkill dressage) || dressage.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Dressage - Maîtrise", CustomSkill.AnimalHandlingProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.ArcanaProficiency, out LearnableSkill arcane) || arcane.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Arcanes - Maîtrise", CustomSkill.ArcanaProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.AthleticsProficiency, out LearnableSkill athle) || athle.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Athlétisme - Maîtrise", CustomSkill.AthleticsProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.DeceptionProficiency, out LearnableSkill deception) || deception.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Tromperie - Maîtrise", CustomSkill.DeceptionProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.HistoryProficiency, out LearnableSkill histoire) || histoire.currentLevel < 1)
                skillList.Add(new NuiComboEntry("History - Maîtrise", CustomSkill.HistoryProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.InsightProficiency, out LearnableSkill intuition) || intuition.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Intuition - Maîtrise", CustomSkill.InsightProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.IntimidationProficiency, out LearnableSkill intimidation) || intimidation.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Intimidation - Maîtrise", CustomSkill.IntimidationProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.InvestigationProficiency, out LearnableSkill investigation) || investigation.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Investigation - Maîtrise", CustomSkill.InvestigationProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.MedicineProficiency, out LearnableSkill medecine) || medecine.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Médecine - Maîtrise", CustomSkill.MedicineProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.NatureProficiency, out LearnableSkill nature) || nature.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Nature - Maîtrise", CustomSkill.NatureProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.PerceptionProficiency, out LearnableSkill perc) || perc.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Perception - Maîtrise", CustomSkill.PerceptionProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.PerformanceProficiency, out LearnableSkill perf) || perf.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Performance - Maîtrise", CustomSkill.PerformanceProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.PersuasionProficiency, out LearnableSkill persu) || persu.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Persuasion - Maîtrise", CustomSkill.PersuasionProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.ReligionProficiency, out LearnableSkill religion) || religion.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Religion - Maîtrise", CustomSkill.ReligionProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.SleightOfHandProficiency, out LearnableSkill escamotage) || escamotage.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Escamotage - Maîtrise", CustomSkill.SleightOfHandProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.StealthProficiency, out LearnableSkill stealth) || stealth.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Furtivité - Maîtrise", CustomSkill.StealthProficiency));

              if (!learnableSkills.TryGetValue(CustomSkill.SurvivalProficiency, out LearnableSkill surv) || surv.currentLevel < 1)
                skillList.Add(new NuiComboEntry("Survie - Maîtrise", CustomSkill.SurvivalProficiency));

              break;
          }

          if (skillList.Count < 1)
            return;

          if(skillList.Count == 1)
          {
            int selection = skillList[0].Value;

            learnableSkills.TryAdd(selection, new LearnableSkill((LearnableSkill)learnableDictionary[selection], this));
            learnableSkills[selection].LevelUp(this);
            learnableSkills[selection].source.Add(Utils.In(source, CustomRace.Drow, CustomRace.HighElf, CustomRace.WoodElf, CustomRace.Human) ? Category.Race : Category.Class);

            oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Delete();
            oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Delete();
            return;
          }

          await NwTask.NextFrame();

          if (!windows.TryGetValue("skillBonusChoice", out var value)) windows.Add("skillBonusChoice", new SkillBonusChoiceWindow(this, oid.LoginCreature.Level, source, option, skillList));
          else ((SkillBonusChoiceWindow)value).CreateWindow(oid.LoginCreature.Level, source, option, skillList);
        }
      }
    }
  }
}
