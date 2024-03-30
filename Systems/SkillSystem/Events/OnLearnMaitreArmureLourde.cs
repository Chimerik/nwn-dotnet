using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreArmureLourde(PlayerSystem.Player player, int customSkillId)
    {
      byte rawStrength = player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength);
      if (rawStrength < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(rawStrength + 1));

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MaitreArmureLourde))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MaitreArmureLourde);      

      return true;
    }
  }
}
