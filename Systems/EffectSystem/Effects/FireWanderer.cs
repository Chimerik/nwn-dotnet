using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FireWandererEffectTag = "_FIRE_WANDERER_EFFECT";
    public static Effect FireWanderer
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Fire, 50);
        eff.Tag = FireWandererEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
