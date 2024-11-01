using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFouleeRafraichissante(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FouleeRafraichissante))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FouleeRafraichissante);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante, (byte)(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 1 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1));

      return true;
    }
  }
}
