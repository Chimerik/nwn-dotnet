using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void InitializeBonusSkillChoice()
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

                  if (!learnableSkills.TryGetValue(CustomSkill.NatureProficiency, out LearnableSkill nature) || nature.currentLevel < 1)
                    skillList.Add(new NuiComboEntry("Nature - Maîtrise", CustomSkill.NatureProficiency));

                  break;

                case SkillConfig.SkillOptionType.Cantrip:

                  if (!learnableSkills.TryGetValue(CustomSkill.ArcaneArcherPrestidigitation, out LearnableSkill prestidigitation) || prestidigitation.currentLevel < 1)
                    skillList.Add(new NuiComboEntry("Prestidigitation - Tour de magie", CustomSkill.ArcaneArcherPrestidigitation));

                  if (!learnableSkills.TryGetValue(CustomSkill.ArcaneArcherDruidisme, out LearnableSkill druidisme) || druidisme.currentLevel < 1)
                    skillList.Add(new NuiComboEntry("Druidisme - Tour de magie", CustomSkill.ArcaneArcherDruidisme));

                  break;

                case SkillConfig.SkillOptionType.ArcaneShot:

                  foreach (var arcaneShot in SkillSystem.learnableDictionary.Values.Where(l => ((LearnableSkill)l).category == SkillSystem.Category.TirArcanique))
                    if(!learnableSkills.TryGetValue(arcaneShot.id, out LearnableSkill learnable) || learnable.currentLevel < 1)
                      skillList.Add(new NuiComboEntry(arcaneShot.name, arcaneShot.id));

                  break;
              }

              break;
          }

          if (skillList.Count < 1)
            return;

          if (!windows.TryGetValue("skillBonusChoice", out var value)) windows.Add("skillBonusChoice", new SkillBonusChoiceWindow(this, oid.LoginCreature.Level, source, option, skillList));
          else ((SkillBonusChoiceWindow)value).CreateWindow(oid.LoginCreature.Level, source, option, skillList);
        }
      }
    }
  }
}
