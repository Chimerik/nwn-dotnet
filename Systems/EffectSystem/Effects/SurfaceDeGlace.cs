﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SurfaceDeGlaceEffectTag = "_SURFACE_DE_GLACE_EFFECT";
    private static ScriptCallbackHandle onEnterSurfaceDeGlaceCallback;
    private static ScriptCallbackHandle onExitSurfaceDeGlaceCallback;
    private static ScriptCallbackHandle onHeartbeatSurfaceDeGlaceCallback;
    public static Effect SurfaceDeGlace(float aoeSize)
    {
      var vfxType = aoeSize switch
      {
        _ => (PersistentVfxType)258,// = 2 m
      };

      Effect eff = Effect.AreaOfEffect(vfxType, onEnterSurfaceDeGlaceCallback, onHeartbeatSurfaceDeGlaceCallback, onExitSurfaceDeGlaceCallback);
      eff.Tag = SurfaceDeGlaceEffectTag;
      return eff;
    }
    private static ScriptHandleResult onEnterSurfaceDeGlace(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Entering is NwCreature entering && eventData.Effect.Creator is NwCreature caster)
      {
        ApplyTerrainDifficileEffect(entering, caster, NwSpell.FromSpellId(CustomSpell.TempeteDeNeige));
        ApplyKnockdown(entering, caster, CustomSkill.AcrobaticsProficiency, Ability.Dexterity, 12);
      }
      
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatSurfaceDeGlace(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) && eventData.Effect.Creator is NwCreature caster)
      {
        foreach (NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
        {
          ApplyTerrainDifficileEffect(entering, caster, NwSpell.FromSpellId(CustomSpell.TempeteDeNeige));

          if (entering.MovementType != MovementType.Stationary)
            ApplyKnockdown(entering, caster, CustomSkill.AcrobaticsProficiency, Ability.Dexterity, 12);
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitSurfaceDeGlace(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(exiting, eventData.Effect.Creator, CustomSpell.TempeteDeNeige, TerrainDifficileEffectTag);
 
      return ScriptHandleResult.Handled;
    }
  }
}
