using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VulnerabiliteElecEffectTag = "_VULNERABILITE_ELEC_EFFECT";
    public static Effect VulnerabiliteElec
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ElectricalVulnerability);
        eff.Tag = VulnerabiliteElecEffectTag;
        return eff;
      }
    }
  }
}
