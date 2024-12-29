using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RetraiteExpeditiveEffectTag = "_RETRAITE_EXPEDITIVE_EFFECT";
    public static Effect RetraiteExpeditive
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = RetraiteExpeditiveEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
