using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFrenziedStrike(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BersekerFrenziedStrike);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BersekerFrenziedStrike, 0);

      return true;
    }
  }
}
