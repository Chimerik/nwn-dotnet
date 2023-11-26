using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CrossbowMasterEffectTag = "_CROSSBOW_MASTER_EFFECT";
    public static readonly Native.API.CExoString CrossBowMasterExoTag = "_CROSSBOW_MASTER_EFFECT".ToExoString();
    public static Effect crossbowMasterEffect
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
