using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static readonly string ProvocationEffectTag = "_PROVOCATION_EFFECT";
    public static readonly Native.API.CExoString provocationEffectExoTag = ProvocationEffectTag.ToExoString();
    public static Effect provocation
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.Taunted), Effect.VisualEffect(VfxType.DurMindAffectingNegative),
          Effect.RunAction());
        eff.Tag = ProvocationEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
