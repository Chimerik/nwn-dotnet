using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPacteDeLaLame(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PacteDeLaLame))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.PacteDeLaLame);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PacteDeLaLameInvoquer))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.PacteDeLaLameInvoquer);

      return true;
    }
  }
}
