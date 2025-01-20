using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceForceEffectTag = "_RESISTANCE_FORCE_EFFECT";
    public static Effect ResistanceForce
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ForceResistance);
        eff.Tag = ResistanceForceEffectTag;
        return eff;
      }
    }
  }
}
