using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AmeRadieuseEffectTag = "_AME_RADIEUSE_EFFECT";
    public static Effect AmeRadieuse
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Divine, 50);
        eff.Tag = AmeRadieuseEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
