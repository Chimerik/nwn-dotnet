﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EnchevetrementEffectTag = "_ENCHEVETREMENT_EFFECT";
    private static ScriptCallbackHandle onEnterEnchevetrementCallback;
    private static ScriptCallbackHandle onExitEnchevetrementCallback;
    public static Effect Enchevetrement(NwCreature caster)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerEntangle, onEnterEnchevetrementCallback, onExitHandle: onExitEnchevetrementCallback);
      eff.Tag = EnchevetrementEffectTag;
      eff.Creator = caster;
      return eff;
    }
    private static ScriptHandleResult onEnterEnchevetrement(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Entering is NwCreature entering && eventData.Effect.Creator is NwCreature caster)
        ApplyTerrainDifficileEffect(entering);
      
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitEnchevetrement(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(exiting, TerrainDifficileEffectTag);
 
      return ScriptHandleResult.Handled;
    }
  }
}
