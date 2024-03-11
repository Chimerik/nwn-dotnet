using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DesertionEffectTag = "_DESERTION_EFFECT";
    public static Effect Desertion
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurEtherealVisage), Effect.Invisibility(InvisibilityType.Normal), 
          Effect.DamageImmunityIncrease((DamageType)CustomDamageType.Necrotic, 50), Effect.DamageImmunityIncrease((DamageType)CustomDamageType.Psychic, 50),
          Effect.DamageImmunityIncrease(DamageType.Acid, 50), Effect.DamageImmunityIncrease(DamageType.Fire, 50), Effect.DamageImmunityIncrease(DamageType.Sonic, 50),
          Effect.DamageImmunityIncrease(DamageType.Cold, 50), Effect.DamageImmunityIncrease(DamageType.Electrical, 50), Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50),
          Effect.DamageImmunityIncrease(DamageType.Divine, 50), Effect.DamageImmunityIncrease(DamageType.Negative, 50), Effect.DamageImmunityIncrease(DamageType.Positive, 50),
          Effect.DamageImmunityIncrease(DamageType.Piercing, 50), Effect.DamageImmunityIncrease(DamageType.Slashing, 50));
        
        eff.Tag = DesertionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
