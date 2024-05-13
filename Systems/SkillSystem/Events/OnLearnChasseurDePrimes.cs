using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnChasseurDePrimes(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerChasseurDePrimes))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.RangerChasseurDePrimes);

      if(player.learnableSkills.TryAdd(CustomSkill.InvestigationProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvestigationProficiency], player)))
        player.learnableSkills[CustomSkill.InvestigationProficiency].LevelUp(player);
      player.learnableSkills[CustomSkill.InvestigationProficiency].source.Add(Category.Class);

      return true;
    }
  }
}
