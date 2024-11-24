using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreBouclier(PlayerSystem.Player player, int customSkillId)
    {
      byte rawStr = player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength);
      if (rawStr < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(rawStr + 1));

      if (!player.oid.LoginCreature.KnowsFeat(Feat.ShieldProficiency))
        player.oid.LoginCreature.AddFeat(Feat.ShieldProficiency);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MaitreBouclier))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MaitreBouclier);

      return true;
    }
  }
}
