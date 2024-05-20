using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TrueStrikeEffectTag = "_TRUE_STRIKE_EFFECT";
    public static readonly Native.API.CExoString trueStrikeEffectExoTag = TrueStrikeEffectTag.ToExoString();
    public static Effect TrueStrike
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackIncrease);
        eff.Tag = TrueStrikeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
