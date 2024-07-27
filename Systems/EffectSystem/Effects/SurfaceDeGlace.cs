using System.Linq;
using Anvil.API;
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
    public static Effect SurfaceDeGlace(int aoeSize)
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
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Entering is NwCreature entering)
      {
        ApplyTerrainDifficileEffect(entering);

        if (CreatureUtils.HandleSkillCheck(entering, CustomSkill.AcrobaticsProficiency, Ability.Dexterity, 12))
          ApplyKnockdown(entering, CreatureSize.Large, 2);
      }
      
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatSurfaceDeGlace(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData))
        return ScriptHandleResult.Handled;

      foreach (NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        ApplyTerrainDifficileEffect(entering);

        if (entering.MovementType != MovementType.Stationary
            && CreatureUtils.HandleSkillCheck(entering, CustomSkill.AcrobaticsProficiency, Ability.Dexterity, 12))
          ApplyKnockdown(entering, CreatureSize.Large, 2);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitSurfaceDeGlace(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(exiting, TerrainDifficileEffectTag);
 
      return ScriptHandleResult.Handled;
    }
  }
}
