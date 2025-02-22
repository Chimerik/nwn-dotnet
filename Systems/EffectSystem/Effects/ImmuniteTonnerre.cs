

using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteTonnerreEffectTag = "_IMMUNITE_TONNERRE_EFFECT";
    public static Effect ImmuniteTonnerre
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.TonnerreImmunity);
        eff.Tag = ImmuniteTonnerreEffectTag;
        return eff;
      }
    }
  }
}
