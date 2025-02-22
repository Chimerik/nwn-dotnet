using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeBrutaleEffectTag = "_FRAPPE_BRUTALE_EFFECT";
    public static Effect FrappeBrutale(int featId)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.FrappeBrutale);
      eff.Tag = FrappeBrutaleEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = featId;
      return eff;
    }
  }
}
