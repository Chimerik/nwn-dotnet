using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string boneChillEffectTag = "_BONE_CHILL_EFFECT";
    public static readonly Native.API.CExoString boneChillEffectExoTag = "_BONE_CHILL_EFFECT".ToExoString();
    public static Effect boneChillEffect
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurBigbysInterposingHand);
        eff.Tag = boneChillEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
