using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AcidAffinityEffectTag = "_ACID_AFFINITY_EFFECT";
    public static Effect AcidAffinity
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Acid, 50);
        eff.Tag = AcidAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
