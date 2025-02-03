using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BonusActionEffectTag = "_BONUS_ACTIONS_EFFECT";
    public const int BonusActionId = -1;
    private static ScriptCallbackHandle onRemoveBonusActionCallback;
    public static void ApplyActionBonus(NwCreature creature)
    {
      if(creature.ActiveEffects.Any(e => e.Tag == NoBonusActionEffectTag))
      {
        creature.ApplyEffect(EffectDuration.Temporary, Cooldown(creature, 6, BonusActionId), NwTimeSpan.FromRounds(1));
      }
      else
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.ActionBonus), Effect.RunAction(onRemovedHandle: onRemoveBonusActionCallback));
        eff.Tag = BonusActionEffectTag;
        eff.SubType = EffectSubType.Unyielding;

        creature.ApplyEffect(EffectDuration.Permanent, eff);
      }
    }
    private static ScriptHandleResult OnRemoveBonusAction(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.ApplyEffect(EffectDuration.Temporary, Cooldown(creature, 6, BonusActionId), TimeSpan.FromSeconds(5.9));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
