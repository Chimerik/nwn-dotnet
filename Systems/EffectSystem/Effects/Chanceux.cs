using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChanceuxAvantageEffectTag = "_CHANCEUX_AVANTAGE_EFFECT";
    public const string ChanceuxDesavantageEffectTag = "_CHANCEUX_DESAVANTAGE_EFFECT";
    public static Effect ChanceuxAvantage
    {
      get
      {
        Effect link = Effect.Icon(CustomEffectIcon.Chanceux);
        link.Tag = ChanceuxAvantageEffectTag;
        link.SubType = EffectSubType.Supernatural;

        return link;
      }
    }

    public static Effect ChanceuxDesavantage
    {
      get
      {
        Effect link = Effect.Icon(CustomEffectIcon.Chanceux);
        link.Tag = ChanceuxDesavantageEffectTag;
        link.SubType = EffectSubType.Supernatural;

        return link;
      }
    }
  }
}
