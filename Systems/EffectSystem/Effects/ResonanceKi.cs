using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResonanceKiEffectTag = "_RESONANCE_KI_EFFECT";
    public static Effect ResonanceKi
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurEtherealVisage);
        eff.Tag = ResonanceKiEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
