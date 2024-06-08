using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AbjurationWardEffectTag = "_ABJURATION_WARD_EFFECT";
    public static readonly Native.API.CExoString abjurationWardEffectExoTag = AbjurationWardEffectTag.ToExoString();
    public static Effect GetAbjurationWardEffect(int intensity)
    {
      
      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.DamageReduction), Effect.DamageReduction(intensity, DamagePower.Plus20));
      eff.CasterLevel = intensity;
      eff.Tag = AbjurationWardEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
