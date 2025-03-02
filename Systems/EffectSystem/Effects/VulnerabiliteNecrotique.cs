using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteNecrotiqueEffectTag = "_VULNERABILITE_NECROTIQUE_EFFECT";
    public static Effect VulnerabiliteNecrotique
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.NecrotiqueVulnerability);
        eff.Tag = VulnerabiliteNecrotiqueEffectTag;
        return eff;
      }
    }
  }
}
