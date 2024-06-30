using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRenvoiDesMortsVivants(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.TurnUndead))
        player.oid.LoginCreature.AddFeat(Feat.TurnUndead);

      player.oid.LoginCreature.SetFeatRemainingUses(Feat.TurnUndead, 1);

      return true;
    }
  }
}
