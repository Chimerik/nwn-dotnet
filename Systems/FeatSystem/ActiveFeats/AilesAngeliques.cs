using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AilesAngeliques(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (caster.WingType == CreatureWingType.None)
      {
          caster.WingType = CreatureWingType.Angel;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseHoly));
      }
      else
      {
        caster.WingType = CreatureWingType.None;
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));
      }
    }
  }
}
