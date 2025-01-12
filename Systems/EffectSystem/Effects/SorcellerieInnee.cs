using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SorcellerieInneeEffectTag = "_SORCELLERIE_INNEE_EFFECT";
    public static Effect SorcellerieInnee
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurGlowLightOrange);
        eff.Tag = SorcellerieInneeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
