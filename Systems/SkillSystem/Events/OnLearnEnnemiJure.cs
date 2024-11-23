using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnInfatiguable(PlayerSystem.Player player, int customSkillId)
    {
      NwCreature creature = player.oid.LoginCreature;

      if (!creature.KnowsFeat((Feat)customSkillId))
        creature.AddFeat((Feat)customSkillId);

      creature.SetFeatRemainingUses((Feat)customSkillId, (byte)(creature.GetAbilityModifier(Ability.Wisdom) > 1 ? creature.GetAbilityModifier(Ability.Wisdom) : 1));

      return true;
    }
  }
}
