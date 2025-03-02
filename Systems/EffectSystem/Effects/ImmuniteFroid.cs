using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteFroidEffectTag = "_IMMUNITE_FROID_EFFECT";
    public static Effect ImmuniteFroid
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ColdImmunity);
        eff.Tag = ImmuniteFroidEffectTag;
        return eff;
      }
    }
  }
}
