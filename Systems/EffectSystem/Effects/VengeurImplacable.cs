using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VengeurImplacableEffectTag = "_VENGEUR_IMPLACABLE_EFFECT";
    public static Effect VengeurImplacable
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(25);
        eff.Tag = VengeurImplacableEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
