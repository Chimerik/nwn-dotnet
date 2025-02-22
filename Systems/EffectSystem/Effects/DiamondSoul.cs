using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DiamondSoulEffectTag = "_DIAMOND_SOUL_EFFECT";
    public static Effect DiamondSoul
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.AmeDeDiamant);
        eff.Tag = DiamondSoulEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
