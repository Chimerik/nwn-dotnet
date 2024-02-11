using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalSaignementCallback;
    private static ScriptCallbackHandle onRemoveSaignementCallback;
    public const string SaignementEffectTag = "_EFFECT_SAIGNEMENT";
    public static readonly Native.API.CExoString saignementEffectExoTag = SaignementEffectTag.ToExoString();
    public static Effect saignementEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.RunAction(onRemovedHandle: onRemoveSaignementCallback, onIntervalHandle: onIntervalSaignementCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = SaignementEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalSaignement(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(2, DamageType.Slashing), Effect.VisualEffect(VfxType.ComBloodSparkLarge)));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveSaignement(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.OnHeal -= OnHealRemoveSaignement;

      return ScriptHandleResult.Handled;
    }
    private static void OnHealRemoveSaignement(OnHeal onHeal)
    {
      EffectUtils.RemoveTaggedEffect(onHeal.Target, SaignementEffectTag);
    }
  }
}
