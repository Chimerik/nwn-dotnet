using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceFroidEffectTag = "_RESISTANCE_FROID_EFFECT";
    public static Effect ResistanceFroid
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ColdResistance);
        eff.Tag = ResistanceFroidEffectTag;
        return eff;
      }
    }
  }
}
