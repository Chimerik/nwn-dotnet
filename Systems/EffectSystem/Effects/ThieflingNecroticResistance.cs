using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ThieflingNecroticResistanceEffectTag = "_THIEFLING_NECROTIC_RESISTANCE_EFFECT";
    public static Effect ThieflingNecroticResistance
    {
      get
      {
        Effect eff = ResistanceNecrotique;
        eff.Tag = ThieflingNecroticResistanceEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
