using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Native.API;
using EffectSubType = Anvil.API.EffectSubType;
using ImmunityType = Anvil.API.ImmunityType;
using RacialType = Anvil.API.RacialType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onApplyCharmCallback;
    private static ScriptCallbackHandle onRemoveCharmCallback;
    public const string CharmEffectTag = "_CHARM_EFFECT";
    public static Effect Charme
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingDisabled), Effect.RunAction(onAppliedHandle: onApplyCharmCallback, onRemovedHandle: onRemoveCharmCallback));
        eff.Tag = CharmEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect GetCharmImmunityEffect(string effectTag)
    {
      Effect eff = Effect.LinkEffects(Effect.Immunity(ImmunityType.Charm));
      eff.Tag = effectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    public static bool IsCharmeImmune(NwCreature caster, NwCreature target)
    {
      if (target.ActiveEffects.Any(e => e.EffectType == EffectType.Immunity && e.IntParams[1] == 14)
        || (Utils.In(caster.Race.RacialType, RacialType.Fey, RacialType.Aberration, RacialType.Outsider, RacialType.Elemental, RacialType.Undead)
        && target.ActiveEffects.Any(e => e.Tag == ProtectionContreLeMalEtLeBienEffectTag)))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} dispose d'une immunité contre les effets de charme");
        return true; 
      }

      return false;
    }
    public static bool IsCharmeImmune(CNWSCreature caster, CNWSCreature target)
    {
      if (target.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Immunity && e.GetInteger(1) == 28)
        || Utils.In((RacialType)caster.m_pStats.m_nRace, RacialType.Fey, RacialType.Aberration, RacialType.Outsider, RacialType.Elemental, RacialType.Undead))
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
      EffectUtils.RemoveTaggedEffect(onDamage.Creature, CharmEffectTag);
    }
    [ScriptHandler("on_charm_attack")]
    private void OnCharmAttack(CallInfo callInfo)
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
