using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RespirationAquatiqueEffectTag = "_RESPIRATION_AQUATIQUE_EFFECT";
    public static Effect RespirationAquatique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)189), Effect.VisualEffect(VfxType.DurMindAffectingPositive), Effect.RunAction());
        eff.Tag = RespirationAquatiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
