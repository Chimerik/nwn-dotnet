using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FurieElementaireEffectTag = "_FURIE_ELEMENTAIRE_EFFECT";
    public static readonly Native.API.CExoString FurieElementaireEffectExoTag = FurieElementaireEffectTag.ToExoString();
    public static Effect FurieElementaire(DamageBonus damage, DamageType damageType)
    {
      Effect eff = Effect.DamageIncrease((int)damage, damageType);
      eff.Tag = FurieElementaireEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
