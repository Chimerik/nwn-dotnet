using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappePrimordialeEffectTag = "_FRAPPE_PRIMORDIALE_EFFECT";
    public static readonly Native.API.CExoString FrappePrimordialeEffectExoTag = FrappePrimordialeEffectTag.ToExoString();
    public static Effect FrappePrimordiale(DamageBonus damage, DamageType damageType)
    {
      Effect eff = Effect.DamageIncrease((int)damage, damageType);
      eff.Tag = FrappePrimordialeEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
