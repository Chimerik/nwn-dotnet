using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ToileDaraigneeEffectTag = "_TOILE_DARAIGNEE_EFFECT";
    private static ScriptCallbackHandle onEnterToileDaraigneeCallback;
    private static ScriptCallbackHandle onExitToileDaraigneeCallback;
    private static ScriptCallbackHandle oHeartbeatToileDaraigneeCallback;
    public static Effect ToileDaraignee(NwCreature caster)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerWeb, onEnterToileDaraigneeCallback, oHeartbeatToileDaraigneeCallback, onExitToileDaraigneeCallback);
      eff.Tag = ToileDaraigneeEffectTag;
      eff.Creator = caster;
      return eff;
    }
    private static ScriptHandleResult onEnterToileDaraignee(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Entering is NwCreature entering && eventData.Effect.Creator is NwCreature caster)
      {
        ApplyTerrainDifficileEffect(entering, caster, CustomSpell.ToileDaraignee);

        Ability castingAbility = (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value;
        SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.ToileDaraignee];
        int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellType(Spell.Web), castingAbility);

        if (CreatureUtils.GetSavingThrow(caster, entering, castingAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
          Entrave(entering, caster, castingAbility, eventData.Effect.RemainingDuration, true);
      }
      
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitToileDaraignee(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting && eventData.Effect.Creator is NwCreature caster)
        EffectUtils.RemoveTaggedEffect(exiting, caster, CustomSpell.ToileDaraignee, TerrainDifficileEffectTag);
 
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatToileDaraignee(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) && eventData.Effect.Creator is NwCreature caster)
      {
        foreach (NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
        {
          if (entering.ActiveEffects.Any(e => e.Tag == EffectSystem.EntraveEffectTag))
            continue;

          Ability castingAbility = (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value;
          SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.ToileDaraignee];
          int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellType(Spell.Web), castingAbility);

          if (CreatureUtils.GetSavingThrow(caster, entering, castingAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
            Entrave(entering, caster, castingAbility, eventData.Effect.RemainingDuration, true);
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
