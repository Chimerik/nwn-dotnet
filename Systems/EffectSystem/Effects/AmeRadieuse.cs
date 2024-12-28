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
        Effect res = Effect.DamageImmunityIncrease(DamageType.Divine, 50);
        res.ShowIcon = false;

        Effect eff = Effect.LinkEffects(res, Effect.Icon(CustomEffectIcon.RadiantResistance));
        eff.Tag = AmeRadieuseEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
