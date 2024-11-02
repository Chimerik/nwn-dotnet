using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFaveurDuMalin(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FaveurDuMalin))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FaveurDuMalin);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FaveurDuMalin, (byte)(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 1 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1));

      return true;
    }
  }
}
