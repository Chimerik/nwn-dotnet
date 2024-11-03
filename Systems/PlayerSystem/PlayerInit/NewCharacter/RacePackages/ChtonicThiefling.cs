using Anvil.API;
using NWN.Core;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyChtonicPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Thaumaturgy, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Thaumaturgy], this)))
          learnableSkills[CustomSkill.Thaumaturgy].LevelUp(this);
        learnableSkills[CustomSkill.Thaumaturgy].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.MageHand, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MageHand], this)))
          learnableSkills[CustomSkill.MageHand].LevelUp(this);
        learnableSkills[CustomSkill.MageHand].source.Add(Category.Race);

        oid.LoginCreature.TailType = CreatureTailType.Devil;

        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingNecroticResistance));
      }
    }
  }
}
