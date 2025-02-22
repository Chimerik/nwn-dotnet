using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string InspirationBardiqueEffectTag = "_INSPIRATION_BARDIQUE_EFFECT";
    public static readonly Native.API.CExoString inspirationBardiqueEffectExoTag = InspirationBardiqueEffectTag.ToExoString();
    public static Effect GetInspirationBardiqueEffect(int bonus)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.InspirationBardique);
      eff.Tag = InspirationBardiqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = bonus;
      return eff;
    }

    public static int GetInspirationBardiqueBonus(int bardLevel)
    {
      int inspirationBardique = NwRandom.Roll(Utils.random, bardLevel > 14 ? 12 : bardLevel > 9 ? 10 : bardLevel > 4 ? 8 : 6);

      LogUtils.LogMessage($"Inspiration bardique - Bard level {bardLevel} : {inspirationBardique}", LogUtils.LogType.Combat);

      return inspirationBardique;
    }
  }
}
