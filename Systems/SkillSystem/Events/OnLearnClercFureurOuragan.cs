using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnClercFureurOuraganFoudre(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)customSkillId, CreatureUtils.GetAbilityModifierMin1(player.oid.LoginCreature, Ability.Wisdom));

      return true;
    }
  }
}
