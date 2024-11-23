using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Infatiguable(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, 8) + CreatureUtils.GetAbilityModifierMin1(caster, Ability.Wisdom)));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.RangerInfatiguable);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Infatiguable", StringUtils.gold, true, true);
    }
  }
}
