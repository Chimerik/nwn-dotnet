using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnActor(PlayerSystem.Player player, int customSkillId)
    {
      byte rawCharisma = player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma);
      if (rawCharisma < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Charisma, (byte)(rawCharisma + 1));

      return true;
    }
  }
}
