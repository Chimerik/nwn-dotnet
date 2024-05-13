using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ColdWandererEffectTag = "_COLD_WANDERER_EFFECT";
    public static Effect ColdWanderer
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Cold, 50);
        eff.Tag = ColdWandererEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
