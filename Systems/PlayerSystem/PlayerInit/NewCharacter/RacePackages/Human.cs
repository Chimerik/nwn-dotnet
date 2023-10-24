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
        if (learnableSkills.TryAdd(CustomSkill.LightArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightArmorProficiency])))
          learnableSkills[CustomSkill.LightArmorProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LightArmorProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.ShieldProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShieldProficiency])))
          learnableSkills[CustomSkill.ShieldProficiency].LevelUp(this);

        learnableSkills[CustomSkill.ShieldProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.SpearProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SpearProficiency])))
          learnableSkills[CustomSkill.SpearProficiency].LevelUp(this);

        learnableSkills[CustomSkill.SpearProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HumanVersatility, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HumanVersatility])))
          learnableSkills[CustomSkill.HumanVersatility].LevelUp(this);

        learnableSkills[CustomSkill.HumanVersatility].source.Add(Category.Race);

        if (bonusSelection > -1)
        {
          if (learnableSkills.TryAdd(Utils.skillList[bonusSelection].Value, new LearnableSkill((LearnableSkill)learnableDictionary[Utils.skillList[bonusSelection].Value])))
            learnableSkills[Utils.skillList[bonusSelection].Value].LevelUp(this);

          learnableSkills[Utils.skillList[bonusSelection].Value].source.Add(Category.Race);
        }
        
        oid.LoginCreature.OnAcquireItem -= ItemSystem.OnAcquireCheckHumanVersatility;
        oid.LoginCreature.OnUnacquireItem -= ItemSystem.OnUnAcquireCheckHumanVersatility;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireCheckHumanVersatility;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnAcquireCheckHumanVersatility;
      }
    }
  }
}
