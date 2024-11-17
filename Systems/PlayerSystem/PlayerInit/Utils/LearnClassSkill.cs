using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void LearnClassSkill(int learnableId)
      {
        if(learnableSkills.TryAdd(learnableId, new LearnableSkill((LearnableSkill)learnableDictionary[learnableId], this)));
          learnableSkills[learnableId].LevelUp(this);
        learnableSkills[learnableId].source.Add(Category.Class);
      }
    }
  }
}
