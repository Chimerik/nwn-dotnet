using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceAcideEffectTag = "_RESISTANCE_ACIDE_EFFECT";
    public static Effect ResistanceAcide
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.AcidResistance);
        eff.Tag = ResistanceAcideEffectTag;
        return eff;
      }
    }
  }
}
