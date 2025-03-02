using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteForceEffectTag = "_VULNERABILITE_FORCE_EFFECT";
    public static Effect VulnerabiliteForce
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ForceVulnerability);
        eff.Tag = VulnerabiliteForceEffectTag;
        return eff;
      }
    }
  }
}
