using System;
using System.Linq;
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
    public const string EspritsGardienCooldownTag = "_ESPRITS_GARDIENS_COOLDOWN_EFFECT";
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
    public static Effect EspritsGardiensCooldown
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = EspritsGardienCooldownTag;
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

      if (!entering.ActiveEffects.Any(e => e.Tag == EspritsGardienCooldownTag && e.Creator == protector))
      {
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));
        entering.ApplyEffect(EffectDuration.Temporary, EspritsGardiensCooldown, TimeSpan.FromSeconds(5));

        EffectUtils.RemoveEffectType(protector, EffectType.Invisibility, EffectType.ImprovedInvisibility);
        NwSpell spell = eventData.Effect.Spell;
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
        int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, Ability.Wisdom);

        SpellUtils.DealSpellDamage(entering, protector.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(protector, spell), protector, 3, 
          CreatureUtils.GetSavingThrow(protector, entering, spellEntry.savingThrowAbility, spellDC, spellEntry));
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
        if (target == caster || !caster.IsReactionTypeHostile(target) || target.ActiveEffects.Any(e => e.Tag == EspritsGardienCooldownTag && e.Creator == caster))
          continue;

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));
        target.ApplyEffect(EffectDuration.Temporary, EspritsGardiensCooldown, TimeSpan.FromSeconds(5));

        SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, 3, 
          CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
