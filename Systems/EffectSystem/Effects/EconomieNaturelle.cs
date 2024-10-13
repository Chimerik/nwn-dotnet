using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EconomieNaturelleEffectTag = "_ECONOMIE_NATURELLE_EFFECT";
    public static Effect EconomieNaturelle
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)184);
        eff.Tag = EconomieNaturelleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
