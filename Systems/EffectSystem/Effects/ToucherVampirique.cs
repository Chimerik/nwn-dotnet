using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ToucherVampiriqueEffectTag = "_TOUCHER_VAMPIRIQUE_EFFECT";
    public static Effect ToucherVampirique
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ToucherVampirique);
        eff.Tag = ToucherVampiriqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
