using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AvatarDeBatailleEffectTag = "_AVATAR_DE_BATAILLE_EFFECT";
    public static Effect AvatarDeBataille
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50),
          Effect.DamageImmunityIncrease(DamageType.Piercing, 50), Effect.DamageImmunityIncrease(DamageType.Slashing, 50));
        eff.Tag = AvatarDeBatailleEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
