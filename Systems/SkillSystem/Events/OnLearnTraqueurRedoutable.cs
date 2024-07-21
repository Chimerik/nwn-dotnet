using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTraqueurRedoutable(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TraqueurRedoutable))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.TraqueurRedoutable);

      player.oid.OnCombatStatusChange -= RangerUtils.OnCombatTraqueurRedoutable;
      player.oid.OnCombatStatusChange += RangerUtils.OnCombatTraqueurRedoutable;

      player.learnableSkills.TryAdd(CustomSkill.TraqueurLinceulDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TraqueurLinceulDombre], player));
      player.learnableSkills[CustomSkill.TraqueurLinceulDombre].LevelUp(player);
      player.learnableSkills[CustomSkill.TraqueurLinceulDombre].source.Add(Category.Class);

      return true;
    }
  }
}
