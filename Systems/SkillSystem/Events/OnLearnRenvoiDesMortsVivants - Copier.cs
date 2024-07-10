using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnCharmePlanteEtAnimaux(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercCharmePlanteEtAnimaux))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercCharmePlanteEtAnimaux);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercCharmePlanteEtAnimaux, 1);

      return true;
    }
  }
}
