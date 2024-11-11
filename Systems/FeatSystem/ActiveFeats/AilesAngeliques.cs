using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AilesAngeliques(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.WingType = caster.WingType == CreatureWingType.None ? CreatureWingType.Angel : CreatureWingType.None;
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseHoly));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHoly));
    }
  }
}
