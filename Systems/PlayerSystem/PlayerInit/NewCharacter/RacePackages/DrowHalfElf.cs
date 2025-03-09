using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDrowHalfElfPackage()
      { 
        if (learnableSkills.TryAdd(CustomSkill.Profond, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Profond], this)))
          learnableSkills[CustomSkill.Profond].LevelUp(this);
        learnableSkills[CustomSkill.Profond].source.Add(Category.Race);

        oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique, 1);
        oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomRace.HighHalfElf;
        InitializeBonusSkillChoice();
      }
    }
  }
}
