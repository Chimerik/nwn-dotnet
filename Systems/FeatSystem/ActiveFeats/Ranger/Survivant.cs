using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Survivant(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(CreatureUtils.GetAbilityModifierMin1(caster, Ability.Constitution)));
    }
  }
}
