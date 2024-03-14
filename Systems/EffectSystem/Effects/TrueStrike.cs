using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string trueStrikeEffectTag = "_TRUE_STRIKE_EFFECT";
    public static readonly Native.API.CExoString trueStrikeEffectExoTag = trueStrikeEffectTag.ToExoString();
    public static Effect trueStrikeEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurCessatePositive), Effect.Icon(EffectIcon.AttackIncrease));
        eff.Tag = trueStrikeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
