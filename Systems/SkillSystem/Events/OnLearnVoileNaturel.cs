


using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnVoileNaturel(PlayerSystem.Player player, int customSkillId)
    {
      NwCreature creature = player.oid.LoginCreature;

      if (!creature.KnowsFeat((Feat)customSkillId))
        creature.AddFeat((Feat)customSkillId);

      creature.SetFeatRemainingUses((Feat)customSkillId, CreatureUtils.GetAbilityModifierMin1(creature, Ability.Wisdom));

      return true;
    }
  }
}
