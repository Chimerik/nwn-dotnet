using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LameDoutretombeEffectTag = "_LAME_DOUTRETOMBE_EFFECT";
    public static Effect lameDoutretombe
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ACIncrease(1), Effect.Icon(CustomEffectIcon.LameDoutretombe));
        eff.Tag = LameDoutretombeEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
