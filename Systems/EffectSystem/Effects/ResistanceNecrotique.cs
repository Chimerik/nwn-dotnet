using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceNecrotiqueEffectTag = "_RESISTANCE_NECROTIQUE_EFFECT";
    public static Effect ResistanceNecrotique
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.NecrotiqueResistance);
        eff.Tag = ResistanceNecrotiqueEffectTag;
        return eff;
      }
    }
  }
}
