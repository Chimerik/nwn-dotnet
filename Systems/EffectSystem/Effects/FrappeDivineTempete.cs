using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDivineTempeteEffectTag = "_FRAPPE_DIVINE_TEMPETE_EFFECT";
    public static readonly Native.API.CExoString FrappeDivineTempeteEffectExoTag = FrappeDivineTempeteEffectTag.ToExoString();
    public static Effect GetFrappeDivineTempeteEffect(DamageBonus damage)
    {
      Effect eff = Effect.DamageIncrease((int)damage, DamageType.Sonic);
      eff.Tag = FrappeDivineTempeteEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
