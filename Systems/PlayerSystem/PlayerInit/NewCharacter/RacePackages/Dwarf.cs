using System.Linq;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDwarfPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Nain, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Nain], this)))
          learnableSkills[CustomSkill.Nain].LevelUp(this);

        learnableSkills[CustomSkill.Nain].source.Add(Category.Race);

        oid.LoginCreature.LevelInfo.FirstOrDefault().HitDie += 1;
        NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.DwarfPoisonResistance));
      }
    }
  }
}
