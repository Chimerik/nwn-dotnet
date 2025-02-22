using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurDestructriceEffectTag = "_FUREUR_DESTRUCTRICE_EFFECT";
    public static Effect FureurDestructrice
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.FureurDestructrice);
        eff.Tag = FureurDestructriceEffectTag;
        return eff;
      }
    }
  }
}
