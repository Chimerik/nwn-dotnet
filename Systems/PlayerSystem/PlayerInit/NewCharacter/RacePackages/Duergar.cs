using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDuergarPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.LightHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightHammerProficiency])))
          learnableSkills[CustomSkill.LightHammerProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LightHammerProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarHammerProficiency])))
          learnableSkills[CustomSkill.WarHammerProficiency].LevelUp(this);

        learnableSkills[CustomSkill.WarHammerProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HandAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HandAxeProficiency])))
          learnableSkills[CustomSkill.HandAxeProficiency].LevelUp(this);

        learnableSkills[CustomSkill.HandAxeProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.WarAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarAxeProficiency])))
          learnableSkills[CustomSkill.WarAxeProficiency].LevelUp(this);

        learnableSkills[CustomSkill.WarAxeProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Profond, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Profond])))
          learnableSkills[CustomSkill.Profond].LevelUp(this);

        learnableSkills[CustomSkill.Profond].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);

        // TODO : Penser à gérer l'avantage sur les JDS contre les illusions, les charmes et la paralysie
      }
    }
  }
}
