using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoisonEffectTag = "_POISON_EFFECT";
    public static readonly Native.API.CExoString poisonEffectExoTag = PoisonEffectTag.ToExoString();
    private static ScriptCallbackHandle onIntervalPoisonCallback;
    public static SavingThrowResult ApplyPoison(NwCreature target, NwCreature caster, TimeSpan duration, Ability SaveAbility, Ability DCAbility = Ability.Dexterity, bool repeatSave = false, bool noSave = false)
    {
      if (IsPoisonImmune(target, caster))
        return SavingThrowResult.Immune;

      Effect eff = repeatSave ? Effect.LinkEffects(Effect.Icon(EffectIcon.Poison), Effect.RunAction(onIntervalHandle: onIntervalPoisonCallback, interval: NwTimeSpan.FromRounds(1))) : Effect.Icon(EffectIcon.Poison);
      eff.Tag = PoisonEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.IntParams[5] = (int)DCAbility;

      if (noSave)
      {
        target.ApplyEffect(EffectDuration.Temporary, eff, duration);
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
        return SavingThrowResult.Failure;
      }

      int spellDC = SpellUtils.GetCasterSpellDC(caster, DCAbility);
      SavingThrowResult result = CreatureUtils.GetSavingThrow(caster, target, SaveAbility, spellDC, effectType: SpellConfig.SpellEffectType.Poison);

      if (result == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Temporary, eff, duration);
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
      }

      return result;
    }
    public static bool IsPoisonImmune(NwCreature target, NwCreature caster)
    {
      if (Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct) || target.ActiveEffects.Any(e => e.EffectType == EffectType.Immunity && e.IntParams[1] == 2))
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

      if (eventData.EffectTarget is NwCreature target && eff.Creator is NwCreature caster)
      {
        int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)eff.IntParams[5]);

        if (CreatureUtils.GetSavingThrow(caster, target, Ability.Constitution, spellDC, effectType: SpellConfig.SpellEffectType.Poison) != SavingThrowResult.Failure)
          target.RemoveEffect(eventData.Effect);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
