
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTireurDelite(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TireurDelite))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.TireurDelite);

      byte rawDex = player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity);
      if (rawDex < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(rawDex + 1));

      return true;
    }
  }
}
