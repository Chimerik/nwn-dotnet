using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoisonEffectTag = "_POISON_EFFECT";
    public static readonly Native.API.CExoString poisonEffectExoTag = PoisonEffectTag.ToExoString();
    public static Effect Poison
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.Poison);
        eff.Tag = PoisonEffectTag;
        return eff;
      }
    }
  }
}
