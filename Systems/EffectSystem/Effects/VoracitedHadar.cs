using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VoracitedHadarEffectTag = "_VORACITE_DE_HADAR_EFFECT";
    private static ScriptCallbackHandle onEnterVoracitedHadarCallback;
    private static ScriptCallbackHandle onHeartbeatVoracitedHadarCallback;
    private static ScriptCallbackHandle onExitVoracitedHadarCallback;
    public static Effect VoracitedHadar(NwGameObject caster, NwSpell spell)
    {
      Effect eff = Effect.AreaOfEffect(CustomAoE.VoracitedHadar, onEnterSphereDeFeuCallback, onHeartbeatSphereDeFeuCallback, onExitVoracitedHadarCallback);
      eff.Tag = VoracitedHadarEffectTag;
      eff.Creator = caster;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterVoracitedHadar(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.VoraciteDhadar);
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      ApplyTerrainDifficileEffect(entering, protector, NwSpell.FromSpellId(CustomSpell.VoraciteDhadar));

      int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);
      HandleVoracitedHadarEffect(protector, entering, spellEntry, spell, spellDC);

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult onHeartbeatVoracitedHadar(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData)
        && eventData.Effect.Creator is NwCreature caster)
      {
        NwSpell spell = NwSpell.FromSpellId(CustomSpell.SphereDeFeu).MasterSpell;
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

        int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);

        foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
          HandleVoracitedHadarEffect(caster, target, spellEntry, spell, spellDC);
      }

      return ScriptHandleResult.Handled;
    }

    private static void HandleVoracitedHadarEffect(NwCreature caster, NwCreature target, SpellEntry spellEntry, NwSpell spell, int spellDC)
    {
      SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, spell.InnateSpellLevel);

      if(CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry) == SavingThrowResult.Failure)
        SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, spell.InnateSpellLevel, forcedDamage:DamageType.Acid);

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameM));
    }

    private static ScriptHandleResult onExitVoracitedHadar(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, CustomSpell.VoraciteDhadar, TerrainDifficileEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
