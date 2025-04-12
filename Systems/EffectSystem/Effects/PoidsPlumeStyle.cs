using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoidsPlumeStyleEffectTag = "_POIDS_PLUME_STYLE_EFFECT";
    public static Effect PoidsPlumeStyle
    {
      get
      {
        Effect link = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.PoidsPlumeStyle), 
          Effect.MovementSpeedIncrease(15),
          Effect.DamageIncrease(1, DamageType.Piercing));

        link.Tag = PoidsPlumeStyleEffectTag;
        link.SubType = EffectSubType.Unyielding;

        return link;
      }
    }
  }
}
