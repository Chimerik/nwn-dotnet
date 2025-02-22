using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AmeDesVentsEffectTag = "_AME_DES_VENTS_EFFECT";
    public static Effect AmeDesVents
    {
      get
      {
        Effect eff = Effect.LinkEffects(ImmuniteElec, ImmuniteTonnerre);
        eff.Tag = AmeDesVentsEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
