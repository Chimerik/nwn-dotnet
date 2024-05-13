using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AcidWandererEffectTag = "_ACID_WANDERER_EFFECT";
    public static Effect AcidWanderer
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Acid, 50);
        eff.Tag = AcidWandererEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
