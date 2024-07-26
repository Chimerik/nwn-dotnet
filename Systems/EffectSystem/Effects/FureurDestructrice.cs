using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurDestructriceEffectTag = "_FUREUR_DESTRUCTRICE_EFFECT";
    public static readonly Native.API.CExoString FureurDestructriceExoTag = FureurDestructriceEffectTag.ToExoString();
    public static Effect FureurDestructrice
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
        eff.Tag = FureurDestructriceEffectTag;
        return eff;
      }
    }
  }
}
