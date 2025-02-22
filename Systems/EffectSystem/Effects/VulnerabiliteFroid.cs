using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteFroidEffectTag = "_VULNERABILITE_FROID_EFFECT";
    public static Effect VulnerabiliteFroid
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ColdVulnerability);
        eff.Tag = VulnerabiliteFroidEffectTag;
        return eff;
      }
    }
  }
}
