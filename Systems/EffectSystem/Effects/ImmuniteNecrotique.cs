using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteNecrotiqueEffectTag = "_IMMUNITE_NECROTIQUE_EFFECT";
    public static Effect ImmuniteNecrotique
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.NecrotiqueImmunity);
        eff.Tag = ImmuniteNecrotiqueEffectTag;
        return eff;
      }
    }
  }
}
