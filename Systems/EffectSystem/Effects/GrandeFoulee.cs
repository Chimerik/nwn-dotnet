using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string GrandeFouleeEffectTag = "_GRANDE_FOULEE_EFFECT";
    public static Effect GrandeFoulee
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(15);
        eff.Tag = GrandeFouleeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
