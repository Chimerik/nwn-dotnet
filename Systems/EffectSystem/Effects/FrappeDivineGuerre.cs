using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDivineGuerreEffectTag = "_FRAPPE_DIVINE_GUERRE_EFFECT";
    public static readonly Native.API.CExoString FrappeDivineGuerreEffectExoTag = FrappeDivineGuerreEffectTag.ToExoString();
    public static Effect GetFrappeDivineGuerreEffect(DamageBonus damage, DamageType damageType)
    {
      Effect eff = Effect.DamageIncrease((int)damage, damageType);
      eff.Tag = FrappeDivineGuerreEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
