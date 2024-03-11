using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkPatienceEffectTag = "_MONK_PATIENCE_EFFECT";
    public static readonly Native.API.CExoString MonkPatienceEffectExoTag = MonkPatienceEffectTag.ToExoString();
    public static Effect MonkPatience
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurCessatePositive);
        eff.Tag = MonkPatienceEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
