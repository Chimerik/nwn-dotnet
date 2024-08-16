using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ColdAffinityEffectTag = "_COLD_AFFINITY_EFFECT";
    public static Effect ColdAffinity
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Cold, 50);
        eff.Tag = ColdAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
