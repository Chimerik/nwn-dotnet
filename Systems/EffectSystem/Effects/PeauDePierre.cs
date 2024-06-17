using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PeauDePierreEffectTag = "_PEAU_DE_PIERRE_EFFECT";
    public static Effect PeauDePierre
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Slashing, 50),
          Effect.DamageImmunityIncrease(DamageType.Piercing, 50), Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50));
        eff.Tag = PeauDePierreEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
