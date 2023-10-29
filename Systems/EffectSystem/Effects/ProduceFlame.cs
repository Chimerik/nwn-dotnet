using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect produceFlameEffect
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurAuraRedLight);
        eff.Tag = ProduceFlameEffectTag;
        return eff;
      }
    }
    public const string ProduceFlameEffectTag = "_PRODUCE_FLAME_EFFECT";
  }
}
