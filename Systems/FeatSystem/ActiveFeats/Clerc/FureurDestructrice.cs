using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FureurDestructrice(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FureurDestructrice);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
