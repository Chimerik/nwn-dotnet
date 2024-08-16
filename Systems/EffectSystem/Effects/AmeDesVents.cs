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
        Effect eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Electrical, 100), 
          Effect.DamageImmunityIncrease(DamageType.Sonic, 100));
        eff.Tag = AmeDesVentsEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
