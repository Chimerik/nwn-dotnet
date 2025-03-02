using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabilitePsychiqueEffectTag = "_VULNERABILITE_PSYCHIQUE_EFFECT";
    public static Effect VulnerabilitePsychique
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PsychiqueVulnerability);
        eff.Tag = VulnerabilitePsychiqueEffectTag;
        return eff;
      }
    }
  }
}
