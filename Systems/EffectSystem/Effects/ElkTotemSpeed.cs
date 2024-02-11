using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ElkTotemSpeedEffectTag = "_ELK_TOTEM_SPEED_EFFECT";
    public static Effect elkTotemSpeed
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(25);
        eff.Tag = ElkTotemSpeedEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
