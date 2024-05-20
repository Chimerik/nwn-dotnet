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

      player.learnableSkills.TryAdd(CustomSkill.MonkLinceulDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkLinceulDombre], player));
      player.learnableSkills[CustomSkill.MonkLinceulDombre].LevelUp(player);
      player.learnableSkills[CustomSkill.MonkLinceulDombre].source.Add(Category.Class);

      return true;
    }
  }
}
