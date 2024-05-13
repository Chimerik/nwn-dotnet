using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoisonWandererEffectTag = "_POISON_WANDERER_EFFECT";
    public static Effect PoisonWanderer
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
        eff.Tag = PoisonWandererEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
