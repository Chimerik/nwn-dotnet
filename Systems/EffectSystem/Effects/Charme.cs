using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Native.API;
using EffectSubType = Anvil.API.EffectSubType;
using Feat = Anvil.API.Feat;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onApplyCharmCallback;
    private static ScriptCallbackHandle onRemoveCharmCallback;
    public const string CharmEffectTag = "_CHARM_EFFECT";
    public static Effect charmEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative), Effect.RunAction(onAppliedHandle: onApplyCharmCallback, onRemovedHandle: onRemoveCharmCallback));
        eff.Tag = CharmEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static bool IsCharmeImmune(NwCreature target)
    {
      if (target.KnowsFeat((Feat)CustomSkill.BersekerRageAveugle)
        && target.ActiveEffects.Any(e => e.Tag == BarbarianRageEffectTag))
        return true;

      return false;
    }
    public static bool IsCharmeImmune(CNWSCreature target)
    {
      if (target.m_pStats.HasFeat(CustomSkill.BersekerRageAveugle).ToBool()
        && target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(barbarianRageEffectExoTag).ToBool()))
        return true;

      return false;
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

        target.OnDamaged -= OnDamageCharmed;
        target.OnDamaged += OnDamageCharmed;

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
      target.OnDamaged -= OnDamageCharmed;
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "on_charm_attack", target);

      return ScriptHandleResult.Handled;
    }
    public static void OnDamageCharmed(CreatureEvents.OnDamaged onDamage)
    {
      foreach (var eff in onDamage.Creature.ActiveEffects.Where(e => e.Tag == CharmEffectTag))
        onDamage.Creature.RemoveEffect(eff);
    }
    [ScriptHandler("on_charm_attack")]
    private void OnCombatEnter(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is not NwCreature creature)
        return;

      if(creature.ActiveEffects.Any(e => e.Tag == CharmEffectTag 
        && NWScript.StringToObject(EventsPlugin.GetEventData("TARGET")) == e.Creator))
      {
        EventsPlugin.SkipEvent();
        _ = creature.ClearActionQueue();
        creature.LoginPlayer?.SendServerMessage($"Vous êtes sous le charme de cette création et ne pouvez pas la cibler", ColorConstants.Red);

      }
    }
  }
}
