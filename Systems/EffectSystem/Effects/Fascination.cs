using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FascinationEffectTag = "_FASCINATION_EFFECT";
    public static Effect Fascination
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Fascination), Effect.RunAction());
        eff.Tag = FascinationEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

