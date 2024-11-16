using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnEtincelleDivine(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercEtincelleDivine))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercEtincelleDivine);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercEtincelleDivine, 2);

      return true;
    }
  }
}
