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
        if (learnableSkills.TryAdd(CustomSkill.LightArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightArmorProficiency], this)))
          learnableSkills[CustomSkill.LightArmorProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LightArmorProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShieldProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShieldProficiency], this)))
          learnableSkills[CustomSkill.ShieldProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShieldProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.SpearProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SpearProficiency], this)))
          learnableSkills[CustomSkill.SpearProficiency].LevelUp(this);

        learnableSkills[CustomSkill.SpearProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HumanVersatility, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HumanVersatility], this)))
          learnableSkills[CustomSkill.HumanVersatility].LevelUp(this);

        learnableSkills[CustomSkill.HumanVersatility].source.Add(Category.Race);

        if (bonusSelection > -1)
        {
          if (learnableSkills.TryAdd(bonusSelection, new LearnableSkill((LearnableSkill)learnableDictionary[bonusSelection], this)))
            learnableSkills[bonusSelection].LevelUp(this);

          learnableSkills[bonusSelection].source.Add(Category.Race);
        }
        
        oid.LoginCreature.OnAcquireItem -= ItemSystem.OnAcquireCheckHumanVersatility;
        oid.LoginCreature.OnUnacquireItem -= ItemSystem.OnUnAcquireCheckHumanVersatility;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireCheckHumanVersatility;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnAcquireCheckHumanVersatility;
      }
    }
  }
}
