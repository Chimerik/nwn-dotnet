using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TempeteDeNeigeEffectTag = "_TEMPETE_DE_NEIGE_EFFECT";
    public const string TempeteDeNeigeBlindEffectTag = "_TEMPETE_DE_NEIGE_BLIND_EFFECT";
    private static ScriptCallbackHandle onEnterTempeteDeNeigeCallback;
    private static ScriptCallbackHandle onHeartbeatTempeteDeNeigeCallback;
    private static ScriptCallbackHandle onExitTempeteDeNeigeCallback;
    public static Effect TempeteDeNeige(NwGameObject caster, NwSpell spell)
    {
      Effect eff = Effect.AreaOfEffect((PersistentVfxType)61, onEnterTempeteDeNeigeCallback, onExitTempeteDeNeigeCallback);
      eff.Tag = TempeteDeNeigeEffectTag;
      eff.Creator = caster;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }

    public static Effect TempeteDeNeigeBlind
    {
      get
      {
        Effect eff = Effect.Blindness();
        eff.Tag = TempeteDeNeigeBlindEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    private static ScriptHandleResult onEnterTempeteDeNeige(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering)
        return ScriptHandleResult.Handled;

      if(eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      ApplyTerrainDifficileEffect(entering, caster, NwSpell.FromSpellId(CustomSpell.TempeteDeNeige));
      NWScript.AssignCommand(caster, () => entering.ApplyEffect(EffectDuration.Permanent, TempeteDeNeigeBlind));
      TempeteDeNeigeKnockDown(caster, entering, Spells2da.spellTable[CustomSpell.TempeteDeNeige], (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_SPELL_CASTING_ABILITY").Value);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatTempeteDeNeige(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData))
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.TempeteDeNeige];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.TempeteDeNeige), (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_SPELL_CASTING_ABILITY").Value);

      foreach(NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>()) 
      { 
        TempeteDeNeigeKnockDown(caster, entering, spellEntry, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_SPELL_CASTING_ABILITY").Value);
      }

      return ScriptHandleResult.Handled;
    }
    private static void TempeteDeNeigeKnockDown(NwCreature caster, NwCreature entering, SpellEntry spellEntry, Ability DCAbility)
    {
      ApplyKnockdown(entering, caster, DCAbility, spellEntry.savingThrowAbility, Knockdown);
    }

    private static ScriptHandleResult onExitTempeteDeNeige(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, TempeteDeNeigeBlindEffectTag);
      EffectUtils.RemoveTaggedEffect(exiting, protector, CustomSpell.TempeteDeNeige, TerrainDifficileEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
