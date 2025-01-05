using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnIllumination(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercIllumination))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercIllumination);

      int wisMod = CreatureUtils.GetAbilityModifierMin1(player.oid.LoginCreature, Ability.Wisdom);
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercIllumination, (byte)(wisMod > 0 ? wisMod : 1));

      return true;
    }
  }
}
