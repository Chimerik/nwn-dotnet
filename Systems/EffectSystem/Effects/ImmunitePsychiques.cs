using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmunitePsychiqueEffectTag = "_IMMUNITE_PSYCHIQUE_EFFECT";
    public static Effect ImmunitePsychique
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PsychiqueImmunity);
        eff.Tag = ImmunitePsychiqueEffectTag;
        return eff;
      }
    }
  }
}
