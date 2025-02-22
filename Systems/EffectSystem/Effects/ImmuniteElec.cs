using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteElecEffectTag = "_IMMUNITE_ELEC_EFFECT";
    public static Effect ImmuniteElec
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ElectricalImmunity);
        eff.Tag = ImmuniteElecEffectTag;
        return eff;
      }
    }
  }
}
