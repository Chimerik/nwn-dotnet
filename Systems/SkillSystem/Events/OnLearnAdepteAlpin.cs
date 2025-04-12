using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAdepteAlpin(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Escalade(true));

      return true;
    }
  }
}
