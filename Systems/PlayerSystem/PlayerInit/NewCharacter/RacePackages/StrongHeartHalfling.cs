using Anvil.API;
using NWN.Core;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyStrongHeartPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Halfelin, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Halfelin], this)))
          learnableSkills[CustomSkill.Halfelin].LevelUp(this);

        learnableSkills[CustomSkill.Halfelin].source.Add(Category.Race);

        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow));
        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.DwarfPoisonResistance));
      }
    }
  }
}
