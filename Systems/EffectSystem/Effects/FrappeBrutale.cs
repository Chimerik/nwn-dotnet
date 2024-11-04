using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeBrutaleEffectTag = "_FRAPPE_BRUTALE_EFFECT";
    public static readonly Native.API.CExoString FrappeBrutaleEffectExoTag = "_FRAPPE_BRUTALE_EFFECT".ToExoString();
    public static Effect FrappeBrutale(int featId)
    {
      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
      eff.Tag = FrappeBrutaleEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = featId;
      return eff;
    }
  }
}
