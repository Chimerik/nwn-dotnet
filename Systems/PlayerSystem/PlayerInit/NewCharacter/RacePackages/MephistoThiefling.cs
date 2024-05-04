using Anvil.API;
using NWN.Core;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMephistoPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.MageHand, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MageHand], this)))
          learnableSkills[CustomSkill.MageHand].LevelUp(this);

        learnableSkills[CustomSkill.MageHand].source.Add(Category.Race);

        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingFireResistance));
      }
    }
  }
}
