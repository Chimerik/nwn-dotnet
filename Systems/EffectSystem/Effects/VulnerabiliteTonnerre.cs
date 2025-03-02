using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteTonnerreEffectTag = "_VULNERABILITE_TONNERRE_EFFECT";
    public static Effect VulnerabiliteTonnerre
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.TonnerreVulnerability);
        eff.Tag = VulnerabiliteTonnerreEffectTag;
        return eff;
      }
    }
  }
}
