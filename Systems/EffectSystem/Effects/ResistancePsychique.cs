using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistancePsychiqueEffectTag = "_RESISTANCE_PSYCHIQUE_EFFECT";
    public static Effect ResistancePsychique
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PsychiqueResistance);
        eff.Tag = ResistancePsychiqueEffectTag;
        return eff;
      }
    }
  }
}
