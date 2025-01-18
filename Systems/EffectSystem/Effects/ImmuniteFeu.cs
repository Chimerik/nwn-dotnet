using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteFeuEffectTag = "_IMMUNITE_FEU_EFFECT";
    public static Effect ImmuniteFeu
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.FireImmunity);
        eff.Tag = ImmuniteFeuEffectTag;
        return eff;
      }
    }
  }
}
