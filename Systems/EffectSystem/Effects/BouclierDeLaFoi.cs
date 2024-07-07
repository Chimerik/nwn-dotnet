using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BouclierDeLaFoiEffectTag = "_BOUCLIER_DE_LA_FOI_EFFECT";
    public static Effect BouclierDeLaFoi
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ACIncrease(2), Effect.VisualEffect(VfxType.DurProtectionGoodMinor));
        eff.Tag = BouclierDeLaFoiEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
