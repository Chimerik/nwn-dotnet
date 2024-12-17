using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoisonAffinityEffectTag = "_POISON_AFFINITY_EFFECT";
    public static Effect PoisonAffinity
    {
      get
      {
        Effect resist = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
        resist.ShowIcon = false;

        Effect eff = Effect.LinkEffects(resist, Effect.Icon(CustomEffectIcon.PoisonResistance));
        eff.Tag = PoisonAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
