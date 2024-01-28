using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnInstinctSauvage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.UncannyDodge1))
        player.oid.LoginCreature.AddFeat(Feat.UncannyDodge1);

      return true;
    }
  }
}
