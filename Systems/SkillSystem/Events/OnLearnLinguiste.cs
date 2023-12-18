using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnLinguiste(PlayerSystem.Player player, int customSkillId)
    {
      byte rawIntelligence = player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence);
      if (rawIntelligence < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Intelligence, (byte)(rawIntelligence + 1));

      return true;
    }
  }
}
