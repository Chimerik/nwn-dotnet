using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HeroismeEffectTag = "_HEROISME_EFFECT";
    private static ScriptCallbackHandle onIntervalHeroismeCallback;
    public static Effect Heroisme(int modifier)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Heroisme), Effect.Immunity(ImmunityType.Fear), Effect.TemporaryHitpoints(modifier),
        Effect.RunAction(onIntervalHandle: onIntervalHeroismeCallback, interval: NwTimeSpan.FromRounds(1)));
      eff.Tag = HeroismeEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = modifier;
      return eff;
    }
    private static ScriptHandleResult OnIntervalHeroisme(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
        target.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(eventData.Effect.IntParams[5]), NwTimeSpan.FromRounds(1));

      return ScriptHandleResult.Handled;
    }
  }
}
