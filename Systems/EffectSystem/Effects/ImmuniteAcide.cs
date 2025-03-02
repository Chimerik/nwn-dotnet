using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteAcideEffectTag = "_IMMUNITE_ACIDE_EFFECT";
    public static Effect ImmuniteAcide
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.AcidImmunity);
        eff.Tag = ImmuniteAcideEffectTag;
        return eff;
      }
    }
  }
}
