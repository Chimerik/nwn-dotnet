using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFouleeProvocatrice(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FouleeProvocatrice))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FouleeProvocatrice);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FouleeProvocatrice, (byte)(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 1 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1));

      return true;
    }
  }
}
