using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MagieTempetueuseEffectTag = "_MAGIE_TEMPETUEUSE_EFFECT";
    public static Effect MagieTempetueuse
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(25);
        eff.Tag = MagieTempetueuseEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
