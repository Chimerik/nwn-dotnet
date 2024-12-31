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
        Effect bludgeoning = Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50);
        bludgeoning.ShowIcon = false;

        Effect slashing = Effect.DamageImmunityIncrease(DamageType.Slashing, 50);
        slashing.ShowIcon = false;

        Effect piercing = Effect.DamageImmunityIncrease(DamageType.Piercing, 50);
        piercing.ShowIcon = false;

        Effect eff = Effect.LinkEffects(bludgeoning, slashing, piercing, Effect.Icon(CustomEffectIcon.BludgeoningResistance),
          Effect.Icon(CustomEffectIcon.PiercingResistance), Effect.Icon(CustomEffectIcon.SlashingResistance));
        eff.Tag = AvatarDeBatailleEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
