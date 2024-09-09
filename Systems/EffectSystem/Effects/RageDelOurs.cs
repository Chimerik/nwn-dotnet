using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RageDelOursEffectTag = "_RAGE_DE_LOURS_EFFECT";
    public static Effect RageDelOurs
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.AbilityIncrease(Ability.Strength, 2), Effect.AbilityIncrease(Ability.Strength, 2),
          Effect.TemporaryHitpoints(12));
        eff.Tag = RageDelOursEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
