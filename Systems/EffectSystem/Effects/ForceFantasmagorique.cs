using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ForceFantasmagoriqueEffectTag = "_FORCE_FANTASMAGORIQUE_EFFECT";
    private static ScriptCallbackHandle onIntervalForceFantasmagoriqueCallback;
    public static Effect ForceFantasmagorique
    {
      get
      {
        Effect eff = Effect.RunAction(onIntervalHandle: onIntervalCooldownCallback, interval: TimeSpan.FromSeconds(6));
        eff.Tag = ForceFantasmagoriqueEffectTag;
        eff.SubType = EffectSubType.Magical;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.ForceFantasmagorique);
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalForceFantasmagorique(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 6), CustomDamageType.Psychic));
      }

      return ScriptHandleResult.Handled;
    }
  }
}

