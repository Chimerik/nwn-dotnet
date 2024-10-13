using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrightenedEffectTag = "_FRIGHTENED_EFFECT";
    public static readonly Native.API.CExoString frightenedEffectExoTag = "_FRIGHTENED_EFFECT".ToExoString();
    private static ScriptCallbackHandle onRemoveEffroiCallback;
    public static Effect Effroi(NwCreature target)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingFear), Effect.Icon((EffectIcon)183),
        Effect.RunAction(onRemovedHandle:onRemoveEffroiCallback));
      eff.Tag = FrightenedEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.GetObjectVariable<LocalVariableInt>("_PREVIOUS_MOVEMENT_RATE").Value = (int)target.MovementRate;
      target.MovementRate = MovementRate.Immobile;

      return eff;
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
    private static ScriptHandleResult OnRemoveEffroi(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        target.MovementRate = (MovementRate)target.GetObjectVariable<LocalVariableInt>("_PREVIOUS_MOVEMENT_RATE").Value;
        target.GetObjectVariable<LocalVariableInt>("_PREVIOUS_MOVEMENT_RATE").Delete();
      }

      return ScriptHandleResult.Handled;
    }
  }
}
