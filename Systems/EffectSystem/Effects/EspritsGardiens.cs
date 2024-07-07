using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EspritsGardiensEffectTag = "_ESPRITS_GARDIENS_EFFECT";
    public const string EspritsGardiensSlowEffectTag = "_ESPRITS_GARDIENS_SLOW_EFFECT";
    public const string EspritsGardiensCooldown = "_ESPRITS_GARDIENS_COOLDOWN";
    private static ScriptCallbackHandle onEnterEspritsGardiensCallback;
    private static ScriptCallbackHandle onExitEspritsGardiensCallback;
    private static ScriptCallbackHandle onIntervalEspritsGardiensCallback;
    public static Effect EspritsGardiens(DamageType damageType)
    {
      Effect eff = Effect.LinkEffects(Effect.AreaOfEffect((PersistentVfxType)54, onEnterEspritsGardiensCallback, onIntervalEspritsGardiensCallback,
        onExitEspritsGardiensCallback));
      eff.Tag = EspritsGardiensEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    public static Effect EspritsGardiensSlow
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(50);
        eff.Tag = EspritsGardiensSlowEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterEspritsGardiens(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, EspritsGardiensSlow));

      if (entering.GetObjectVariable<DateTimeLocalVariable>(EspritsGardiensCooldown).HasValue
        && DateTime.Compare(entering.GetObjectVariable<DateTimeLocalVariable>(EspritsGardiensCooldown).Value, DateTime.Now) < 0)
      {
        entering.GetObjectVariable<DateTimeLocalVariable>(EspritsGardiensCooldown).Value = DateTime.Now.AddSeconds(5);
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));

        EffectUtils.RemoveEffectType(protector, EffectType.Invisibility, EffectType.ImprovedInvisibility);
        NwSpell spell = eventData.Effect.Spell;
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
        SpellConfig.SavingThrowFeedback feedback = new();
        int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, Ability.Wisdom);
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(entering, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, protector);

        int totalSave = SpellUtils.GetSavingThrowRoll(entering, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(protector, entering, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        SpellUtils.DealSpellDamage(entering, protector.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(protector, spell), protector, 3, saveFailed);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitEspritsGardiens(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || !exiting.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, EspritsGardiensSlowEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onIntervalEspritsGardiens(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveEffectType(caster, EffectType.Invisibility, EffectType.ImprovedInvisibility);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Esprits Gardiens", StringUtils.gold, true, true);

      NwSpell spell = eventData.Effect.Spell;
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, Ability.Wisdom);

      foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        if (target == caster || !caster.IsReactionTypeHostile(target))
          continue;

        if (target.GetObjectVariable<DateTimeLocalVariable>(EspritsGardiensCooldown).HasValue
        && DateTime.Compare(target.GetObjectVariable<DateTimeLocalVariable>(EspritsGardiensCooldown).Value, DateTime.Now) < 0)
        {
          SpellConfig.SavingThrowFeedback feedback = new();
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));
          target.GetObjectVariable<DateTimeLocalVariable>(EspritsGardiensCooldown).Value = DateTime.Now.AddSeconds(5);

          int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, caster);
          int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
          bool saveFailed = totalSave < spellDC;

          SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
          SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, 3, saveFailed);
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
