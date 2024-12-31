using Anvil.API;
using NWN.Core;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAasimarPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Light, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Light], this)))
          learnableSkills[CustomSkill.Light].LevelUp(this);
        learnableSkills[CustomSkill.Light].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.AilesAngeliques, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AilesAngeliques], this)))
          learnableSkills[CustomSkill.AilesAngeliques].LevelUp(this);
        learnableSkills[CustomSkill.AilesAngeliques].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.MainsGuerisseuses, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MainsGuerisseuses], this)))
          learnableSkills[CustomSkill.MainsGuerisseuses].LevelUp(this);
        learnableSkills[CustomSkill.MainsGuerisseuses].source.Add(Category.Race);

        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AasimarResistance));
      }
    }
  }
}
