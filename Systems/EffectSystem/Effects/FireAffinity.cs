using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FireAffinityEffectTag = "_FIRE_AFFINITY_EFFECT";
    public static Effect FireAffinity
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Fire, 50);
        eff.Tag = FireAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
