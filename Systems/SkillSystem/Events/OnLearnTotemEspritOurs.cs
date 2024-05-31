using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemEspritOurs(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TotemFerociteIndomptable))
      {
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.TotemFerociteIndomptable);
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.TotemFerociteIndomptable, 0);
      }

      return true;
    }
  }
}
