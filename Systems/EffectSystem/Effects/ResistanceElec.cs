using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceElecEffectTag = "_RESISTANCE_ELEC_EFFECT";
    public static Effect ResistanceElec
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ElectricalResistance);
        eff.Tag = ResistanceElecEffectTag;
        return eff;
      }
    }
  }
}
