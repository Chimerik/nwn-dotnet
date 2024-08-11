using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ConcentrationAdvantageEffectTag = "_CONCENTRATION_ADVANTAGE_EFFECT";
    public static Effect ConcentrationAdvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = ConcentrationAdvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
