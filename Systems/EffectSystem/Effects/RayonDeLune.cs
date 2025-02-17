using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RayonDeLuneEffectTag = "_RAYON_DE_LUNE_EFFECT";
    private static ScriptCallbackHandle onEnterRayonDeLuneCallback;
    private static ScriptCallbackHandle onExitRayonDeLuneCallback;
    private static ScriptCallbackHandle onHeartbeatRayonDeLuneCallback;
    public static Effect RayonDeLuneAura(NwGameObject caster, NwSpell spell)
    {
      Effect eff = Effect.AreaOfEffect(CustomAoE.RayonDeLune, onEnterRayonDeLuneCallback, onHeartbeatRayonDeLuneCallback, onExitRayonDeLuneCallback);
      eff.Tag = RayonDeLuneEffectTag;
      eff.Creator = caster;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterRayonDeLuneAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.RayonDeLune);
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);
      HandleRayonDeLuneEffect(protector, entering, spellEntry, spell, spellDC);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitRayonDeLuneAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      exiting.OnEffectApply -= RemovePolymorph;
      EffectUtils.RemoveTaggedEffect(exiting, protector, RayonDeLuneEffectTag);

      return ScriptHandleResult.Handled;
    }

    private static void RemovePolymorph(OnEffectApply onEffect)
    {
      if(onEffect.Effect.EffectType == EffectType.Polymorph)
      {
        onEffect.PreventApply = true;

        if (onEffect.Object is NwCreature creature)
          creature.LoginPlayer?.SendServerMessage("Transformation annulée par Rayon de Lune", ColorConstants.Silver);
      }
    }

    private static ScriptHandleResult onHeartbeatRayonDeLuneAura(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) 
        && eventData.Effect.Creator is NwCreature caster)
      {
        NwSpell spell = NwSpell.FromSpellId(CustomSpell.RayonDeLune);
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

        int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);

        foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
          HandleRayonDeLuneEffect(caster, target, spellEntry, spell, spellDC);
      }

      return ScriptHandleResult.Handled;
    }

    private static void HandleRayonDeLuneEffect(NwCreature caster, NwCreature target, SpellEntry spellEntry, NwSpell spell, int spellDC)
    {
      var saveResult = CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry);
      SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, spell.InnateSpellLevel,
      saveResult);

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDivineStrikeHoly));

      if (saveResult == SavingThrowResult.Failure)
      {
        EffectUtils.RemoveEffectType(target, EffectType.Polymorph);

        target.OnEffectApply -= RemovePolymorph;
        target.OnEffectApply += RemovePolymorph;
      }
    }
  }
}
