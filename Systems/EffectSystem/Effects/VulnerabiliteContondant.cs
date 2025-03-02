using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteContondantEffectTag = "_VULNERABILITE_CONTONDANT_EFFECT";
    public static Effect VulnerabiliteContondant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.BludgeoningVulnerability);
        eff.Tag = VulnerabiliteContondantEffectTag;
        return eff;
      }
    }
  }
}
