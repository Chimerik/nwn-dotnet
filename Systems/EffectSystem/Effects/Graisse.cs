using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string GraisseEffectTag = "_GRAISSE_EFFECT";
    private static ScriptCallbackHandle onEnterGraisseCallback;
    private static ScriptCallbackHandle onExitGraisseCallback;
    private static ScriptCallbackHandle onHeartbeatGraisseCallback;
    public static Effect Graisse
    {
      get
      {
        Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerGrease, onEnterGraisseCallback, onHeartbeatGraisseCallback, onExitGraisseCallback);
        eff.Tag = GraisseEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.Graisse);
        return eff;
      }
    }
    private static ScriptHandleResult onEnterGraisse(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Entering is NwCreature entering && eventData.Effect.Creator is NwCreature caster)
      {
        ApplyTerrainDifficileEffect(entering, caster, NwSpell.FromSpellId(CustomSpell.Graisse));
        ApplyKnockdown(entering, caster, (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value, Ability.Dexterity, Knockdown);
      }
      
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatGraisse(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) && eventData.Effect.Creator is NwCreature caster)
      {
        foreach (NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
        {
          ApplyKnockdown(entering, caster, (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value, Ability.Dexterity, Knockdown);
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitGraisse(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(exiting, eventData.Effect.Creator, (int)Spell.Grease, TerrainDifficileEffectTag);
 
      return ScriptHandleResult.Handled;
    }
  }
}
