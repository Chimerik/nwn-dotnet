using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnIncantationPuissante(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercIncantationPuissante))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercIncantationPuissante);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercIncantationPuissante, 0);

      return true;
    }
  }
}
