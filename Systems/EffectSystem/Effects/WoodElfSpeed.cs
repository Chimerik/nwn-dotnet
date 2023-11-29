using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect woodElfSpeed
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(5);
        eff.Tag = woodElfEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public const string woodElfEffectTag = "_WOODELF_SPEED_EFFECT";
  }
}
