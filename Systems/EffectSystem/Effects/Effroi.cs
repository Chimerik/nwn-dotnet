using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrightenedEffectTag = "_FRIGHTENED_EFFECT";
    public static readonly Native.API.CExoString frightenedEffectExoTag = "_FRIGHTENED_EFFECT".ToExoString();
    public static Effect Effroi
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingFear), Effect.CutsceneImmobilize());
        eff.Tag = FrightenedEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static bool IsFrightImmune(NwCreature target, NwCreature caster)
    {
      if (Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct))
        return true;

      if (target.ActiveEffects.Any(e => e.EffectType == EffectType.Immunity && e.IntParams[1] == 28) 
        || (target.KnowsFeat((Feat)CustomSkill.BersekerRageAveugle) && target.ActiveEffects.Any(e => e.Tag == BarbarianRageEffectTag)))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} dispose d'une immunité contre l'effroi");
        return true;
      }

      return false;
    }
  }
}
