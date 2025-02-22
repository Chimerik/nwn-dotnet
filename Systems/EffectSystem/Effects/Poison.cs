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
    public const string PoisonEffectTag = "_POISON_EFFECT";
    private static ScriptCallbackHandle onIntervalPoisonCallback;
    public static SavingThrowResult ApplyPoison(NwCreature target, NwCreature caster, TimeSpan duration, Ability SaveAbility, Ability DCAbility = Ability.Dexterity, bool repeatSave = false, bool noSave = false, int spellId = -1)
    {
      if (IsPoisonImmune(target, caster))
        return SavingThrowResult.Immune;

      int spellDC = SpellUtils.GetCasterSpellDC(caster, DCAbility);
      Effect eff = repeatSave ? Effect.LinkEffects(Effect.Icon(EffectIcon.Poison), Effect.RunAction(onIntervalHandle: onIntervalPoisonCallback, interval: NwTimeSpan.FromRounds(1))) :
        Effect.Icon(EffectIcon.Poison);
      
      if (spellId == CustomSpell.NuageNauseabond)
        eff = Effect.LinkEffects(eff, Effect.Dazed(), Effect.VisualEffect(VfxType.DurMindAffectingDisabled));
      
      eff.Tag = PoisonEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.CasterLevel = spellDC;

      if (noSave)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, eff, duration));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
        return SavingThrowResult.Failure;
      }
      
      SavingThrowResult result = CreatureUtils.GetSavingThrow(caster, target, SaveAbility, spellDC, effectType: SpellConfig.SpellEffectType.Poison);

      if (result == SavingThrowResult.Failure)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, eff, duration));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
      }

      return result;
    }
    public static bool IsPoisonImmune(NwCreature target, NwCreature caster)
    {
      if (Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct) || target.ActiveEffects.Any(e => e.Tag == ImmunitePoisonEffectTag))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} est immunisé au poison");
       
        return true;
      }

      return false;
    }
    private static ScriptHandleResult OnIntervalPoison(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      var eff = eventData.Effect;

      if (eventData.EffectTarget is NwCreature target)
      {
        if (CreatureUtils.GetSavingThrow(eff.Creator is NwGameObject creator ? creator : null, target, Ability.Constitution, eff.CasterLevel, effectType: SpellConfig.SpellEffectType.Poison) != SavingThrowResult.Failure)
          target.RemoveEffect(eventData.Effect);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
