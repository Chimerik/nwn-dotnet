using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDungeonExpert(PlayerSystem.Player player, int customSkillId)
    {
      byte rawCon = player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution);
      if (rawCon < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(rawCon + 1));

      player.oid.LoginCreature.AddFeat(Feat.KeenSense);
      
      return true;
    }
  }
}
