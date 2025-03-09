using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string InspirationHeroiqueEffectTag = "_AASIMAR_RESISTANCE_EFFECT";
    public static Effect InspirationHeroique
    {
      get
      {
        Effect link = Effect.Icon(CustomEffectIcon.InspirationHeroique);

        link.Tag = InspirationHeroiqueEffectTag;
        link.SubType = EffectSubType.Unyielding;

        return link;
      }
    }
  }
}
