using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FleauDinsectesAOEEffectTag = "_FLEAU_DINSECTES_AOE_EFFECT";
    private static ScriptCallbackHandle onEnterFleauDinsectesCallback;
    private static ScriptCallbackHandle onExitFleauDinsectesCallback;
    private static ScriptCallbackHandle onHeartbeatFleauDinsectesCallback;
    public static Effect FleauDinsectesAoE(NwCreature caster, NwSpell spell)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerCreepingDoom, onEnterFleauDinsectesCallback, onHeartbeatFleauDinsectesCallback, onExitFleauDinsectesCallback);
      eff.Tag = FleauDinsectesAOEEffectTag;
      eff.Spell = NwSpell.FromSpellId(CustomSpell.FleauDinsectes);
      eff.Creator = caster;
      eff.Spell = spell;
      return eff;
    }
    private static ScriptHandleResult onEnterFleauDinsectes(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering)
        return ScriptHandleResult.Handled;

      if(eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      ApplyTerrainDifficileEffect(entering, caster, NwSpell.FromSpellId(CustomSpell.FleauDinsectes));

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FleauDinsectes];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.FleauDinsectes), (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value);

      FleauDinsectesDamage(caster, entering, spellEntry, spellDC);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatFleauDinsectes(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData))
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FleauDinsectes];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.FleauDinsectes), (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value);

      foreach(NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>()) 
      {
        FleauDinsectesDamage(caster, entering, spellEntry, spellDC);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitFleauDinsectes(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, eventData.Effect.Creator, CustomSpell.FleauDinsectes, TerrainDifficileEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static void FleauDinsectesDamage(NwCreature caster, NwCreature entering, SpellEntry spellEntry, int spellDC)
    {
      if (CreatureUtils.GetSavingThrow(caster, entering, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
        SpellUtils.DealSpellDamage(entering, caster.CasterLevel, spellEntry, spellEntry.numDice, caster, 5);
    }
  }
}
