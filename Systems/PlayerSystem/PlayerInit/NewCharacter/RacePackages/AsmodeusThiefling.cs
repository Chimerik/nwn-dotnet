using Anvil.API;
using NWN.Core;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAsmodeusPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.ProduceFlame, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ProduceFlame], this)))
          learnableSkills[CustomSkill.ProduceFlame].LevelUp(this);

        learnableSkills[CustomSkill.ProduceFlame].source.Add(Category.Race);

        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ThieflingFireResistance));
      }
    }
  }
}
