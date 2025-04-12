using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string Marcheur3EffectTag = "_MARCHEUR3_EFFECT";

    public static Effect Marcheur3
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(15);
        eff.Tag = Marcheur3EffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    
  }
}
