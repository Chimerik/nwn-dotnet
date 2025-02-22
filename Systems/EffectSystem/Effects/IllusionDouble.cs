using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string IllusionDoubleEffectTag = "_ILLUSION_DOUBLE_EFFECT";
    public static Effect IllusionDouble
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurGhostlyVisageNoSound), Effect.Icon(CustomEffectIcon.IllusionDouble)
          , Effect.RunAction());
        eff.Tag = IllusionDoubleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
