using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeEtourdissanteEffectTag = "_TRUE_STRIKE_EFFECT";
    public static readonly Native.API.CExoString FrappeEtourdissanteEffectExoTag = FrappeEtourdissanteEffectTag.ToExoString();
    public static Effect FrappeEtourdissante
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(50);
        eff.Tag = FrappeEtourdissanteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

