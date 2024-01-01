using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static readonly string FrightenedEffectTag = "_FRIGHTENED_EFFECT";
    public static readonly Native.API.CExoString frightenedEffectExoTag = "_FRIGHTENED_EFFECT".ToExoString();
    public static Effect frighten
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingFear), Effect.CutsceneImmobilize());
        eff.Tag = FrightenedEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
