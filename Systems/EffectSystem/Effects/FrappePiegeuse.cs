﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappePiegeuseEffectTag = "_FRAPPE_PIEGEUSE_EFFECT";
    public const string FrappePiegeuseAttackTag = "_FRAPPE_PIEGEUSE_ATTACK_EFFECT";
    private static ScriptCallbackHandle onIntervalFrappePiegeuseCallback;

    public static Effect FrappePiegeuseAttack
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackIncrease);
        eff.Tag = FrappePiegeuseAttackTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect FrappePiegeuse
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurEntangle), Effect.CutsceneImmobilize(),
          Effect.RunAction(onIntervalHandle: onIntervalCourrouxDeLaNatureCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = FrappePiegeuseEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalFrappePiegeuse(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature caster)
      {
        target.RemoveEffect(eventData.Effect);
        return ScriptHandleResult.Handled;
      }

      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.FrappePiegeuse), Ability.Wisdom);
      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FrappePiegeuse];

      if(CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry) == SavingThrowResult.Failure)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, 
          Effect.Damage(NwRandom.Roll(Utils.random, spellEntry.damageDice, spellEntry.numDice), DamageType.Piercing)));
      else
        target.RemoveEffect(eventData.Effect);

      return ScriptHandleResult.Handled;
    }
  }
}
