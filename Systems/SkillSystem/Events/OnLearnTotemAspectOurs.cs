using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemAspectOurs(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.oid.LoginCreature.OnAcquireItem -= ItemSystem.OnAcquireCheckHumanVersatility;
      player.oid.LoginCreature.OnUnacquireItem -= ItemSystem.OnUnAcquireCheckHumanVersatility;

      player.oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireCheckHumanVersatility;
      player.oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnAcquireCheckHumanVersatility;

      return true;
    }
  }
}
