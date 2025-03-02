using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmunitePerforantEffectTag = "_IMMUNITE_PERFORANT_EFFECT";
    public static Effect ImmunitePerforant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PiercingImmunity);
        eff.Tag = ImmunitePerforantEffectTag;
        return eff;
      }
    }
  }
}
