using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFrappeGuidee(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      int wisMod = CreatureUtils.GetAbilityModifierMin1(player.oid.LoginCreature, Ability.Wisdom);
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercIllumination, (byte)(wisMod > 0 ? wisMod : 1));

      return true;
    }
  }
}
