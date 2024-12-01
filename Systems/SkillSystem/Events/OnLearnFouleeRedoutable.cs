using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFouleeRedoutable(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FouleeRedoutable))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FouleeRedoutable);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FouleeRedoutable, 
        player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante));

      return true;
    }
  }
}
