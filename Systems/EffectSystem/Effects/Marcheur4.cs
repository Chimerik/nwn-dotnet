using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string Marcheur4EffectTag = "_MARCHEUR4_EFFECT";

    public static Effect Marcheur4
    {
      get
      {
        Effect immu = Effect.LinkEffects(Effect.Immunity(ImmunityType.Paralysis), Effect.Immunity(ImmunityType.Entangle),
          Effect.Immunity(ImmunityType.Slow), Effect.Immunity(ImmunityType.MovementSpeedDecrease));

        immu.ShowIcon = false;

        Effect eff = Effect.LinkEffects(immu, Effect.Icon(CustomEffectIcon.LiberteDeMouvement));

        eff.Tag = Marcheur4EffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    
  }
}
