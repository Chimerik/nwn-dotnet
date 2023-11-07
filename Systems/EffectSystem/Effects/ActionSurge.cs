using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ActionSurgeEffectTag = "_ACTION_SURGE_EFFECT";
    public static Effect actionSurgeEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ModifyAttacks(1), Effect.Icon((EffectIcon)154));
        eff.Tag = ActionSurgeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
