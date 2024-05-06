using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnInspirationBardique(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BardInspiration))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BardInspiration);

      int chaMod = player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 0 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1;

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BardInspiration, (byte)chaMod);

      return true;
    }
  }
}
