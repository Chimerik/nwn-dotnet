using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHumanPackage(int bonusSelection)
      {
        if (learnableSkills.TryAdd(CustomSkill.HumanVersatility, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HumanVersatility], this)))
          learnableSkills[CustomSkill.HumanVersatility].LevelUp(this);
        learnableSkills[CustomSkill.HumanVersatility].source.Add(Category.Race);

        oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique, 1);

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomRace.Human;
        InitializeBonusSkillChoice();

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FEAT_SELECTION").Value = 3;

        if (!windows.TryGetValue("featSelection", out var value)) windows.Add("featSelection", new FeatSelectionWindow(this, true));
        else ((FeatSelectionWindow)value).CreateWindow(true);
      }
    }
  }
}
