using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LacerationEffectTag = "_LACERATION_EFFECT";
    public static readonly Native.API.CExoString LacerationEffectExoTag = LacerationEffectTag.ToExoString();
    private static ScriptCallbackHandle onIntervalLacerationCallback;
    public static void Laceration(NwCreature target)
    {
      target.OnHeal -= OnHealRemoveExpertiseEffect;
      target.OnHeal += OnHealRemoveExpertiseEffect;

      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Saignement),
        Effect.RunAction(onIntervalHandle: onIntervalLacerationCallback, interval:NwTimeSpan.FromRounds(1)));
      eff.Tag = LacerationEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(3));
    }
    private static ScriptHandleResult OnIntervalLaceration(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      var eff = eventData.Effect;

      if (eventData.EffectTarget is NwCreature target && eff.Creator is NwCreature caster)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(2, DamageType.Slashing)));
      }

      return ScriptHandleResult.Handled;
    }
  }
}

