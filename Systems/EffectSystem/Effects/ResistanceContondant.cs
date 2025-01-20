using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceContondantEffectTag = "_RESISTANCE_CONTONDANT_EFFECT";
    public static Effect ResistanceContondant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.BludgeoningResistance);
        eff.Tag = ResistanceContondantEffectTag;
        return eff;
      }
    }
  }
}
