using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ActionSurgeEffectTag = "_ACTION_SURGE_EFFECT";
    public static readonly Native.API.CExoString ActionSurgeEffectExoTag = ActionSurgeEffectTag.ToExoString();
    public static Effect ActionSurge
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)154));
        eff.Tag = ActionSurgeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
