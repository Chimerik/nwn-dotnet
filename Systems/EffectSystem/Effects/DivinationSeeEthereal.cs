using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect DivinationSeeEthereal
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMagicalSight));
        eff.Tag = DivinationVisionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
