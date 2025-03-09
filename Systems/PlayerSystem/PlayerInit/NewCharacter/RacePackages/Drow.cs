using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDrowPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Profond, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Profond], this)))
          learnableSkills[CustomSkill.Profond].LevelUp(this);
        learnableSkills[CustomSkill.Profond].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.LightDrow, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightDrow], this)))
          learnableSkills[CustomSkill.LightDrow].LevelUp(this);
        learnableSkills[CustomSkill.LightDrow].source.Add(Category.Race);

        oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);
        ApplyElvenSleepImmunity();

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomRace.Drow;
        InitializeBonusSkillChoice();
      }
    }
  }
}
