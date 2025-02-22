using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SentinelleEffectTag = "_SENTINELLE_EFFECT";
    public static Effect sentinelleEffect
    {
      get
      {
        Effect eff = Effect.CutsceneImmobilize();
        eff.Tag = SentinelleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
