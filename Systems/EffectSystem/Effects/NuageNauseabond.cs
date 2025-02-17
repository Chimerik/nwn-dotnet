using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NuageNauseabondEffectTag = "_NUAGE_NAUSEABOND_EFFECT";
    private static ScriptCallbackHandle onEnterNuageNauseabondCallback;
    private static ScriptCallbackHandle onHeartbeatNuageNauseabondCallback;
    public static Effect NuageNauseabond(NwGameObject caster, NwSpell spell)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerFogstink, onEnterNuageNauseabondCallback, onHeartbeatNuageNauseabondCallback);
      eff.Tag = NuageNauseabondEffectTag;
      eff.Creator = caster;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterNuageNauseabond(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.NuageNauseabond);
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      ApplyPoison(entering, caster, NwTimeSpan.FromRounds(1), spellEntry.savingThrowAbility, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value, spellId: CustomSpell.NuageNauseabond);

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult onHeartbeatNuageNauseabond(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) 
        && eventData.Effect.Creator is NwCreature caster)
      {
        NwSpell spell = NwSpell.FromSpellId(CustomSpell.NuageNauseabond);
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

        foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
          ApplyPoison(target, caster, NwTimeSpan.FromRounds(1), spellEntry.savingThrowAbility, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value, spellId: CustomSpell.NuageNauseabond);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
