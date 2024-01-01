using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onApplyCharmCallback;
    private static ScriptCallbackHandle onRemoveCharmCallback;
    public const string charmEffectTag = "_CHARM_EFFECT";
    public static Effect charmEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative), Effect.RunAction(onAppliedHandle: onApplyCharmCallback, onRemovedHandle: onRemoveCharmCallback));
        eff.Tag = charmEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnApplyCharm(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if(target.IsLoginPlayerCharacter)
      {
        target.OnSpellAction -= SpellSystem.OnSpellInputCharmed;
        target.OnSpellAction += SpellSystem.OnSpellInputCharmed;

        target.OnDamaged -= CreatureUtils.OnDamageCharmed;
        target.OnDamaged += CreatureUtils.OnDamageCharmed;

        // TODO => On damage par l'origine de l'effet, alors on dispel l'effet !

        EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "on_charm_attack", target);
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "on_charm_attack", target);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveCharm(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.OnSpellAction -= SpellSystem.OnSpellInputCharmed;
      target.OnDamaged -= CreatureUtils.OnDamageCharmed;
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "on_charm_attack", target);

      return ScriptHandleResult.Handled;
    }
    [ScriptHandler("on_charm_attack")]
    private void OnCombatEnter(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is not NwCreature creature)
        return;

      if(creature.ActiveEffects.Any(e => e.Tag == charmEffectTag 
        && NWScript.StringToObject(EventsPlugin.GetEventData("TARGET")) == e.Creator))
      {
        EventsPlugin.SkipEvent();
        _ = creature.ClearActionQueue();
        creature.LoginPlayer?.SendServerMessage($"Vous êtes sous le charme de cette création et ne pouvez pas la cibler", ColorConstants.Red);

      }
    }
  }
}
