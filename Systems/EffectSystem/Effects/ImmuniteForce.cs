using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteForceEffectTag = "_IMMUNITE_FORCE_EFFECT";
    public static Effect ImmuniteForce
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ForceImmunity);
        eff.Tag = ImmuniteForceEffectTag;
        return eff;
      }
    }
  }
}
