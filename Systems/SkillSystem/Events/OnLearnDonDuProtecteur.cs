using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDonDuProtecteur(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DonDuProtecteur))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DonDuProtecteur);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.DonDuProtecteur, (byte)(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 1 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1));

      return true;
    }
  }
}
