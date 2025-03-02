
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteAcideEffectTag = "_VULNERABILITE_ACIDE_EFFECT";
    public static Effect VulnerabiliteAcide
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.AcidVulnerability);
        eff.Tag = VulnerabiliteAcideEffectTag;
        return eff;
      }
    }
  }
}
