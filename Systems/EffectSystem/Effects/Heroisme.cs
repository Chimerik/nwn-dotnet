using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HeroismeEffectTag = "_HEROISME_EFFECT";
    private static ScriptCallbackHandle onIntervalHeroismeCallback;
    public static Effect Heroisme(NwSpell spell, int modifier)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Heroisme), Effect.Immunity(ImmunityType.Fear), Effect.TemporaryHitpoints(modifier));
      Effect action = Effect.RunAction(onIntervalHandle: onIntervalHeroismeCallback, interval: NwTimeSpan.FromRounds(1));
      action.CasterLevel = modifier;

      eff = Effect.LinkEffects(eff, action);
      eff.Tag = HeroismeEffectTag;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = modifier;
      return eff;
    }
    private static ScriptHandleResult OnIntervalHeroisme(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
        target.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(eventData.Effect.CasterLevel), NwTimeSpan.FromRounds(1));

      return ScriptHandleResult.Handled;
    }
  }
}
