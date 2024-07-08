using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnLinceulDombre(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercLinceulDombre))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercLinceulDombre);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercLinceulDombre, 1);

      return true;
    }
  }
}
