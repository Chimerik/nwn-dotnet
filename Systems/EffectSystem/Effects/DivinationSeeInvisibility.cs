using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect DivinationSeeInvisibility
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMagicalSight), Effect.SeeInvisible());
        eff.Tag = DivinationVisionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
