using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PresenceIntimidanteUsedEffectTag = "_PRESENCE_INTIMIDANTE_USED_EFFECT";
    
    public static Effect PresenceIntimidanteUsed
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = PresenceIntimidanteUsedEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
