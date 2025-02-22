using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NecroticResistanceEffectTag = "_NECROTIC_RESISTANCE_EFFECT";
    public static Effect NecroticResistance
    {
      get
      {
        Effect eff = ResistanceNecrotique;
        eff.Tag = NecroticResistanceEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
