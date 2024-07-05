using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDivineEffectTag = "_FRAPPE_DIVINE_EFFECT";
    public static readonly Native.API.CExoString FrappeDivineEffectExoTag = FrappeDivineEffectTag.ToExoString();
    public static Effect GetFrappeDivineEffect(DamageBonus damage)
    {
      Effect eff = Effect.DamageIncrease((int)damage, CustomDamageType.Poison);
      eff.Tag = FrappeDivineEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
