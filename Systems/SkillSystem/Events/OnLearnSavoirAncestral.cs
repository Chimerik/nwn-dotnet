using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnSavoirAncestral(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercSavoirAncestral))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercSavoirAncestral);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercSavoirAncestral, 1);

      return true;
    }
  }
}
