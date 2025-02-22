using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ActionSurgeEffectTag = "_ACTION_SURGE_EFFECT";
    public static Effect ActionSurge
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ActionSurge);
        eff.Tag = ActionSurgeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
