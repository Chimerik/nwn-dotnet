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

        if (learnableSkills.TryAdd(CustomSkill.Profond, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Profond], this)))
          learnableSkills[CustomSkill.Profond].LevelUp(this);

        learnableSkills[CustomSkill.Profond].source.Add(Category.Race);
      }
    }
  }
}
