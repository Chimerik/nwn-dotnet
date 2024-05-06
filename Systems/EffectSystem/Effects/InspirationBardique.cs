using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string InspirationBardiqueEffectTag = "_INSPIRATION_BARDIQUE_EFFECT";
    public static readonly Native.API.CExoString inspirationBardiqueEffectExoTag = InspirationBardiqueEffectTag.ToExoString();
    public static Effect GetInspirationBardiqueEffect(int bardLevel)
    {
      Effect eff = Effect.Icon((EffectIcon)169);
      eff.Tag = InspirationBardiqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = NwRandom.Roll(Utils.random, bardLevel > 14 ? 12 : bardLevel > 9 ? 10 : bardLevel > 4 ? 8 : 6);
      return eff;
    }
  }
}
