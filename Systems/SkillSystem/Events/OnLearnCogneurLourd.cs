using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnCogneurLourd(PlayerSystem.Player player, int customSkillId)
    {
      byte rawStr = player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength);
      if (rawStr < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(rawStr + 1));

      if(!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);
      
      return true;
    }
  }
}
