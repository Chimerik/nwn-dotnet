using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string InflexibleEffectTag = "_INFLEXIBLE_EFFECT";
    public static Effect Inflexible
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.Inflexible);
        eff.Tag = InflexibleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
