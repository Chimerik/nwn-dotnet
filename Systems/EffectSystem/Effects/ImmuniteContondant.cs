using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteContondantEffectTag = "_IMMUNITE_CONTONDANT_EFFECT";
    public static Effect ImmuniteContondant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.BludgeoningImmunity);
        eff.Tag = ImmuniteContondantEffectTag;
        return eff;
      }
    }
  }
}
