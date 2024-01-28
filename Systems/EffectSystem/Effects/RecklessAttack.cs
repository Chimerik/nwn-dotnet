using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RecklessAttackEffectTag = "_RECKLESS_ATTACK_EFFECT";
    public static readonly Native.API.CExoString RecklessAttackEffectExoTag = "_RECKLESS_ATTACK_EFFECT".ToExoString();
    public static Effect RecklessAttackEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.ACDecrease), Effect.Icon(EffectIcon.AttackIncrease));
        eff.Tag = RecklessAttackEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
