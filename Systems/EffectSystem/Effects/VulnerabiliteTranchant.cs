
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteTranchantEffectTag = "_VULNERABILITE_TRANCHANT_EFFECT";
    public static Effect VulnerabiliteTranchant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.SlashingVulnerability);
        eff.Tag = VulnerabiliteTranchantEffectTag;
        return eff;
      }
    }
  }
}
