using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnGardienDuVoile(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerVeilProtectionAlignment))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.RangerVeilProtectionAlignment);

      if(player.learnableSkills.TryAdd(CustomSkill.ArcanaProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ArcanaProficiency], player)))
        player.learnableSkills[CustomSkill.ArcanaProficiency].LevelUp(player);
      player.learnableSkills[CustomSkill.ArcanaProficiency].source.Add(Category.Class);

      return true;
    }
  }
}
