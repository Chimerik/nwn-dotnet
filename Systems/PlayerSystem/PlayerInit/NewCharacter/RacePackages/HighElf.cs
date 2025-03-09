using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHighElfPackage(int bonusSelection)
      {
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique], this)))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);
        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Prestidigitation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Prestidigitation], this)))
          learnableSkills[CustomSkill.Prestidigitation].LevelUp(this);
        learnableSkills[CustomSkill.Prestidigitation].source.Add(Category.Race);
        learnableSkills[CustomSkill.Prestidigitation].source.Add(Category.StartingTraits);

        if (learnableSkills.TryAdd(CustomSkill.HighElfCantrip, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HighElfCantrip], this)))
          learnableSkills[CustomSkill.HighElfCantrip].LevelUp(this);
        learnableSkills[CustomSkill.HighElfCantrip].source.Add(Category.Race);

        oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);
        ApplyElvenSleepImmunity();

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomRace.HighElf;
        InitializeBonusSkillChoice();
      }
    }
  }
}
