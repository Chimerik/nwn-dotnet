using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FerociteIndomptable(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(NwRandom.Roll(Utils.random, 8) + caster.GetAbilityModifier(Ability.Constitution)));
    }
  }
}
