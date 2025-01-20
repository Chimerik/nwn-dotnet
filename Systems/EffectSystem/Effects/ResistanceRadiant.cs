using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceRadiantEffectTag = "_RESISTANCE_RADIANT_EFFECT";
    public static Effect ResistanceRadiant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.RadiantResistance);
        eff.Tag = ResistanceRadiantEffectTag;
        return eff;
      }
    }
  }
}
