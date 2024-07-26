using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDivineVieEffectTag = "_FRAPPE_DIVINE_VIE_EFFECT";
    public static readonly Native.API.CExoString FrappeDivineVieEffectExoTag = FrappeDivineVieEffectTag.ToExoString();
    public static Effect GetFrappeDivineVieEffect(DamageBonus damage)
    {
      Effect eff = Effect.DamageIncrease((int)damage, DamageType.Divine);
      eff.Tag = FrappeDivineVieEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
