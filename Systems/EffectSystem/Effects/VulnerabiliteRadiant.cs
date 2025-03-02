using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteRadiantEffectTag = "_VULNERABILITE_RADIANT_EFFECT";
    public static Effect VulnerabiliteRadiant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.RadiantVulnerability);
        eff.Tag = VulnerabiliteRadiantEffectTag;
        return eff;
      }
    }
  }
}
