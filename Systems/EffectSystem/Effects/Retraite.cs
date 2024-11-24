using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RetraiteEffectTag = "_RETRAITE_EFFECT";
    public static Effect Retraite
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(25);
        eff.Tag = RetraiteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

