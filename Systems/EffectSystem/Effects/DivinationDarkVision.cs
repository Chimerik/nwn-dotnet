using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DivinationVisionEffectTag = "_DIVINATION_VISION_EFFECT";
    public static Effect DivinationDarkVision
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurDarkvision), Effect.Ultravision());
        eff.Tag = DivinationVisionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
