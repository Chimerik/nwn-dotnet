using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string trueStrikeEffectTag = "_TRUE_STRIKE_EFFECT";
    public static readonly Native.API.CExoString trueStrikeEffectExoTag = "_TRUE_STRIKE_EFFECT".ToExoString();
    public static Effect trueStrikeEffect
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurCessatePositive);
        eff.Tag = trueStrikeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
