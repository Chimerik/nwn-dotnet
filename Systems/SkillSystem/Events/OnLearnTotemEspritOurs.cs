using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemEspritOurs(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(customSkillId)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(customSkillId));

      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemFerociteIndomptable)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.TotemFerociteIndomptable));

      return true;
    }
  }
}
