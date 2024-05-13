
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRodeurChevalier(PlayerSystem.Player player, int customSkillId)
    {
      if(player.learnableSkills.TryAdd(CustomSkill.HistoryProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HistoryProficiency], player)))
        player.learnableSkills[CustomSkill.HistoryProficiency].LevelUp(player);
      player.learnableSkills[CustomSkill.HistoryProficiency].source.Add(Category.Class);

      return true;
    }
  }
}
