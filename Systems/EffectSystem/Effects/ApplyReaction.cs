using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ReactionEffectTag = "_REACTION_EFFECT";
    public const int ReactionId = -2;
    private static ScriptCallbackHandle onRemoveReactionCallback;
    public static void ApplyReaction(NwCreature creature)
    {
      if(creature.ActiveEffects.Any(e => e.Tag == NoReactionsEffectTag))
      {
        creature.ApplyEffect(EffectDuration.Temporary, Cooldown(creature, 6, ReactionId), NwTimeSpan.FromRounds(1));
      }
      else
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Reaction), Effect.RunAction(onRemovedHandle: onRemoveReactionCallback));
        eff.Tag = ReactionEffectTag;
        eff.SubType = EffectSubType.Unyielding;

        creature.ApplyEffect(EffectDuration.Permanent, eff);
      }
    }
    private static ScriptHandleResult OnRemoveReaction(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.ApplyEffect(EffectDuration.Temporary, Cooldown(creature, 6, ReactionId), NwTimeSpan.FromRounds(1));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
