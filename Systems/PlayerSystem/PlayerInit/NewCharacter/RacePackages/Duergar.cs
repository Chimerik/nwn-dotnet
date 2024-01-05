using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDuergarPackage()
      {
        oid.OnCombatStatusChange -= OnCombatEndRestoreDuergarInvisibility;
        oid.OnCombatStatusChange += OnCombatEndRestoreDuergarInvisibility;

        if (learnableSkills.TryAdd(CustomSkill.LightHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightHammerProficiency], this)))
          learnableSkills[CustomSkill.LightHammerProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LightHammerProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarHammerProficiency], this)))
          learnableSkills[CustomSkill.WarHammerProficiency].LevelUp(this);

        learnableSkills[CustomSkill.WarHammerProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HandAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HandAxeProficiency], this)))
          learnableSkills[CustomSkill.HandAxeProficiency].LevelUp(this);

        learnableSkills[CustomSkill.HandAxeProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.WarAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarAxeProficiency], this)))
          learnableSkills[CustomSkill.WarAxeProficiency].LevelUp(this);

        learnableSkills[CustomSkill.WarAxeProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Profond, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Profond], this)))
          learnableSkills[CustomSkill.Profond].LevelUp(this);

        learnableSkills[CustomSkill.Profond].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);

        // TODO : Penser à gérer l'avantage sur les JDS contre les illusions, les charmes et la paralysie
      }
    }
  }
}
