using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteRadiantEffectTag = "_IMMUNITE_RADIANT_EFFECT";
    public static Effect ImmuniteRadiant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.RadiantImmunity);
        eff.Tag = ImmuniteRadiantEffectTag;
        return eff;
      }
    }
  }
}
