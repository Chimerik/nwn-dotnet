using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnSecondSouffle(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FighterSecondWind))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FighterSecondWind);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FighterSecondWind, 2);

      return true;
    }
  }
}
