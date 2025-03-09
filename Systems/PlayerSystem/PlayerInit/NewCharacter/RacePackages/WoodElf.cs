using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyWoodElfPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique], this)))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);
        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Druidisme, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Druidisme], this)))
          learnableSkills[CustomSkill.Druidisme].LevelUp(this);
        learnableSkills[CustomSkill.Druidisme].source.Add(Category.Race);

        oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);
        ApplyElvenSleepImmunity();
        ApplyWoodElfSpeed();

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomRace.WoodElf;
        InitializeBonusSkillChoice();
      }
    }
  }
}
