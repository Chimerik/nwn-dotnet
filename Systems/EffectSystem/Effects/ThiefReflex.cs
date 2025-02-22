using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ThiefReflexEffectTag = "_THIEF_REFLEX_EFFECT";
    public static readonly Native.API.CExoString ThiefReflexExoTag = ThiefReflexEffectTag.ToExoString();
    public static Effect ThiefReflex
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ThiefReflex);
        eff.Tag = ThiefReflexEffectTag;
        return eff;
      }
    }
  }
}
