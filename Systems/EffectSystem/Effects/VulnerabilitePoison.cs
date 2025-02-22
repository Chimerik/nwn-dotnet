using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabilitePoisonEffectTag = "_VULNERABILITE_POISON_EFFECT";
    public static Effect VulnerabilitePoison
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PoisonVulnerability);
        eff.Tag = VulnerabilitePoisonEffectTag;
        return eff;
      }
    }
  }
}
