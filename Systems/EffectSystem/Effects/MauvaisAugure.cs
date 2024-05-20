using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MauvaisAugureEffectTag = "_MAUVAIS_AUGURE_EFFECT";
    public static readonly Native.API.CExoString mauvaisAugureEffectExoTag = MauvaisAugureEffectTag.ToExoString();
    public static Effect MauvaisAugure
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = MauvaisAugureEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
