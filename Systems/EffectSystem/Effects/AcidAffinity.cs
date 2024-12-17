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
        Effect resist = Effect.DamageImmunityIncrease(DamageType.Acid, 50);
        resist.ShowIcon = false;

        Effect eff = Effect.LinkEffects(resist, Effect.Icon(CustomEffectIcon.AcidResistance));
        eff.Tag = AcidAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
