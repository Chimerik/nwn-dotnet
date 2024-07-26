using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPreservationDeLaVie(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercPreservationDeLaVie))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercPreservationDeLaVie);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercPreservationDeLaVie, 1);

      return true;
    }
  }
}
