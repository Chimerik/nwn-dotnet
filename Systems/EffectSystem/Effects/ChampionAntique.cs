using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChampionAntiqueAuraEffectTag = "_CHAMPION_ANTIQUE_AURA_EFFECT";
    public const string ChampionAntiqueEffectTag = "_CHAMPION_ANTIQUE_EFFECT";
    private static ScriptCallbackHandle onEnterChampionAntiqueCallback;
    private static ScriptCallbackHandle onExitChampionAntiqueCallback;
    private static ScriptCallbackHandle onHeartbeatChampionAntiqueCallback;
    private static ScriptCallbackHandle onRemovedChampionAntiqueCallback;
    public static Effect GetChampionAntiqueEffect(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.AreaOfEffect((PersistentVfxType)185, onEnterHandle: onEnterChampionAntiqueCallback, onExitHandle: onExitChampionAntiqueCallback, heartbeatHandle: onHeartbeatChampionAntiqueCallback),
        Effect.RunAction(onRemovedHandle: onRemovedChampionAntiqueCallback));
      
      eff.Tag = AuraDeGardeEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      caster.OnSpellAction -= PaladinUtils.OnSpellChampionAntique;
      caster.OnSpellAction += PaladinUtils.OnSpellChampionAntique;
      return eff;
    }
    public static Effect ChampionAntique
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SpellResistanceIncrease);
        eff.Tag = GardeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterChampionAntiqueAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, ChampionAntique));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitChampionAntiqueAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || !exiting.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ChampionAntiqueEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatChampionAntiqueAura(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) && eventData.Effect.Creator is NwCreature caster)
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(10)));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onRemoveChampionAntique(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
        target.OnSpellAction -= PaladinUtils.OnSpellChampionAntique;

      return ScriptHandleResult.Handled;
    }
  }
}
