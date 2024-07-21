using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnProtectionLourde(PlayerSystem.Player player, int customSkillId)
    {
      byte rawStrength = player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength);
      if (rawStrength < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(rawStrength + 1));

      player.learnableSkills.TryAdd(CustomSkill.HeavyArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyArmorProficiency], player));
      player.learnableSkills[CustomSkill.HeavyArmorProficiency].source.Add(Category.Feat);

      if (player.learnableSkills[CustomSkill.HeavyArmorProficiency].currentLevel < 1)
        player.learnableSkills[CustomSkill.HeavyArmorProficiency].LevelUp(player);
      
      return true;
    }
  }
}
