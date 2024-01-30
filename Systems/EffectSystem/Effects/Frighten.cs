using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrightenedEffectTag = "_FRIGHTENED_EFFECT";
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
    public static bool IsFrightImmune(NwCreature target)
    {
      if (target.KnowsFeat(NwFeat.FromFeatId(CustomSkill.BersekerRageAveugle))
        && target.ActiveEffects.Any(e => e.Tag == BarbarianRageEffectTag))
        return true;

      return false;
    }
  }
}
