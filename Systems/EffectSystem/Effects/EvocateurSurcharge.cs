using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EvocateurSurchargeEffectTag = "_EVOCATEUR_SURCHARGE_EFFECT";
    public const string EvocateurSurchargeVariable = "_EVOCATEUR_SURCHARGE_VARIABLE";
    public static Effect EvocateurSurcharge
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraPulseOrangeWhite), Effect.Icon((EffectIcon)165));
        eff.Tag = EvocateurSurchargeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
