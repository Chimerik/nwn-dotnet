using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ElectricityAffinityEffectTag = "_ELECTRICITY_AFFINITY_EFFECT";
    public static Effect ElectricityAffinity
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Electrical, 50);
        eff.Tag = ElectricityAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
