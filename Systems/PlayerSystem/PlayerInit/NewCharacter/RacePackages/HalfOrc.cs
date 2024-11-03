using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHalfOrcPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Orc, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Orc], this)))
          learnableSkills[CustomSkill.Orc].LevelUp(this);

        learnableSkills[CustomSkill.Orc].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.enduranceImplacable);
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Value = 1;
        oid.LoginCreature.OnDamaged += CreatureUtils.HandleImplacableEndurance;
      }
    }
  }
}
