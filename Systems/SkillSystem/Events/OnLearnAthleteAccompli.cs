using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAthleteAccompli(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FighterChampionAthleteAccompli))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FighterChampionAthleteAccompli);

      player.oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackAthleteAccompli;
      player.oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackAthleteAccompli;

      return true;
    }
  }
}
