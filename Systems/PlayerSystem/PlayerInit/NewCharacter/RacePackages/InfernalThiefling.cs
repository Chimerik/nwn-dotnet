using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyInfernalPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.FireBolt, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FireBolt], this)))
          learnableSkills[CustomSkill.FireBolt].LevelUp(this);
        learnableSkills[CustomSkill.FireBolt].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Thaumaturgy, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Thaumaturgy], this)))
          learnableSkills[CustomSkill.Thaumaturgy].LevelUp(this);
        learnableSkills[CustomSkill.Thaumaturgy].source.Add(Category.Race);

        oid.LoginCreature.TailType = CreatureTailType.Devil;

        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingFireResistance));
      }
    }
  }
}
