using System;
using System.Linq;
using System.Numerics;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CroissanceDepinesAoEEffectTag = "_CROISSANCE_DEPINES_AOE_EFFECT";
    private static ScriptCallbackHandle onEnterCroissanceDepinesCallback;
    private static ScriptCallbackHandle onExitCroissanceDepinesCallback;
    private static ScriptCallbackHandle onIntervalCroissanceDepinesCallback;
    public static Effect CroissanceDepinesAoE
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.AreaOfEffect(PersistentVfxType.PerEntangle, onEnterHandle: onEnterCroissanceDepinesCallback, onExitHandle: onExitCroissanceDepinesCallback),
          Effect.RunAction(onIntervalHandle: onIntervalCroissanceDepinesCallback, interval: TimeSpan.FromSeconds(1.5)));
        eff.Tag = CroissanceDepinesAoEEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterCroissanceDepines(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Entering is NwCreature entering
        && eventData.Effect.Creator is NwCreature caster)
      {
        ApplyTerrainDifficileEffect(entering, caster, CustomSpell.CroissanceDepines);
        entering.ApplyEffect(EffectDuration.Instant, Effect.Damage((int)DamageBonus.Plus2d4, DamageType.Piercing));
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitCroissanceDepines(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(exiting, eventData.Effect.Creator, CustomSpell.CroissanceDepines, TerrainDifficileEffectTag);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalCroissanceDepines(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwAreaOfEffect aoe)
        return ScriptHandleResult.Handled;

      foreach(var target in aoe.GetObjectsInEffectArea<NwCreature>())
        if(target.MovementType != MovementType.Stationary)
          target.ApplyEffect(EffectDuration.Instant, Effect.Damage((int)DamageBonus.Plus2d4, DamageType.Piercing));

      return ScriptHandleResult.Handled;
    }
  }
}
