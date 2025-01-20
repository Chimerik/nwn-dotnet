using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistancePercantEffectTag = "_RESISTANCE_PERCANT_EFFECT";
    public static Effect ResistancePercant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PiercingResistance);
        eff.Tag = ResistancePercantEffectTag;
        return eff;
      }
    }
  }
}
