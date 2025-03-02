using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteFeuEffectTag = "_VULNERABILITE_FEU_EFFECT";
    public static Effect VulnerabiliteFeu
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.FireVulnerability);
        eff.Tag = VulnerabiliteFeuEffectTag;
        return eff;
      }
    }
  }
}
