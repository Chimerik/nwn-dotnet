using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string IncreaseNumAttackEffectTag = "_INCREASE_NUM_ATTACK_EFFECT";
    public static Effect increaseNumAttackEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ModifyAttacks(1), Effect.Icon(EffectIcon.AttackIncrease));
        eff.Tag = IncreaseNumAttackEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
