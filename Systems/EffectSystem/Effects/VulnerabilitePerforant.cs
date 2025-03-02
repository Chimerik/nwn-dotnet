
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabilitePerforantEffectTag = "_VULNERABILITE_PERFORANT_EFFECT";
    public static Effect VulnerabilitePerforant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PiercingVulnerability);
        eff.Tag = VulnerabilitePerforantEffectTag;
        return eff;
      }
    }
  }
}
