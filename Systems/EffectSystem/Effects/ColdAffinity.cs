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
        Effect resist = Effect.DamageImmunityIncrease(DamageType.Cold, 50);
        resist.ShowIcon = false;

        Effect eff = Effect.LinkEffects(resist, Effect.Icon(CustomEffectIcon.ColdResistance));
        eff.Tag = ColdAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
