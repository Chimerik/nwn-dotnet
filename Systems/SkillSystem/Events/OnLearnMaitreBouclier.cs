using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreBouclier(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.ShieldProficiency))
        player.oid.LoginCreature.AddFeat(Feat.ShieldProficiency);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MaitreBouclier))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MaitreBouclier);

      return true;
    }
  }
}
