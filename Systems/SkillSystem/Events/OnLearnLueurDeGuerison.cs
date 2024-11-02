using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnLueurDeGuerison(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.LueurDeGuérison))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.LueurDeGuérison);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.LueurDeGuérison, (byte)(player.oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Occultiste).Level + 1));

      return true;
    }
  }
}
