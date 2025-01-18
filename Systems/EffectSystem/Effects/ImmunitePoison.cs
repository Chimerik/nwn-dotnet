using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmunitePoisonEffectTag = "_IMMUNITE_POISON_EFFECT";
    public static Effect ImmunitePoison
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PoisonImmunity);
        eff.Tag = ImmunitePoisonEffectTag;
        return eff;
      }
    }
  }
}
