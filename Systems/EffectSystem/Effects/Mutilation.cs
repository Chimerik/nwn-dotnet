using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MutilationEffectTag = "_MUTILATION_EFFECT";
    public static readonly Native.API.CExoString MutilationEffectExoTag = MutilationEffectTag.ToExoString();
    private static ScriptCallbackHandle onRemoveMutilationCallback;
    public static void Mutilation(NwCreature target)
    {
      target.OnHeal -= OnHealRemoveExpertiseEffect;
      target.OnHeal += OnHealRemoveExpertiseEffect;
      target.MovementRate = MovementRate.Immobile;

      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveMutilationCallback);
      eff.Tag = MutilationEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(2));
    }
    private static ScriptHandleResult onRemoveMutilation(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        target.MovementRate = MovementRate.CreatureDefault;
      }

      return ScriptHandleResult.Handled;
    }
  }
}

