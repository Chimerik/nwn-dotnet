using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BrulureEffectTag = "_BRULURE_EFFECT";
    private static ScriptCallbackHandle onIntervalBrulureCallback;
    public static Effect Brulure
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Brulure), Effect.RunAction(onIntervalHandle: onIntervalBrulureCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = BrulureEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalBrulure(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

       target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(NwRandom.Roll(Utils.random, 4), DamageType.Fire),
          Effect.VisualEffect(VfxType.ComHitFire)));

      return ScriptHandleResult.Handled;
    }
  }
}
