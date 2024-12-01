using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFouleeEvanescente(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FouleeEvanescente))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FouleeEvanescente);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FouleeEvanescente, 
        player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante));

      return true;
    }
  }
}
