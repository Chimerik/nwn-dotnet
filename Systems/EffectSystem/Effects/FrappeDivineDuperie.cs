using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDivineDuperieEffectTag = "_FRAPPE_DIVINE_DUPERIE_EFFECT";
    public static readonly Native.API.CExoString FrappeDivineDuperieEffectExoTag = FrappeDivineDuperieEffectTag.ToExoString();
    public static Effect GetFrappeDivineDuperieEffect(DamageBonus damage)
    {
      Effect eff = Effect.DamageIncrease((int)damage, CustomDamageType.Poison);
      eff.Tag = FrappeDivineDuperieEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
