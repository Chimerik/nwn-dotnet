using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DecreaseNumAttackEffectTag = "_DECREASE_NUM_ATTACK_EFFECT";
    public static Effect decreaseNumAttackEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ModifyAttacks(-1), Effect.Icon(EffectIcon.AttackDecrease));
        eff.Tag = DecreaseNumAttackEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
