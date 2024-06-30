using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AngeDeLaVengeanceAuraEffectTag = "_ANGE_DE_LA_VENGEANCE_AURA_EFFECT";
    private static ScriptCallbackHandle onEnterAngeDeLaVengeanceCallback;
    private static ScriptCallbackHandle onIntervalAngeDeLaVengeanceCallback;
    
    public static Effect AngeDeLaVengeance
    {
      get
      {
        Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobDragonFear, onEnterHandle: onEnterAngeDeLaVengeanceCallback, heartbeatHandle: onIntervalAngeDeLaVengeanceCallback);
        eff.Tag = PresenceIntimidanteAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterAngeDeLaVengeance(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature intimidator || intimidator.HP < 1 || entering == intimidator
        || !entering.IsReactionTypeHostile(intimidator) || IsFrightImmune(entering, intimidator))
        return ScriptHandleResult.Handled;

      SpellConfig.SavingThrowFeedback feedback = new();
      int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(intimidator) + intimidator.GetAbilityModifier(Ability.Charisma);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(entering, Ability.Wisdom);
      int totalSave = SpellUtils.GetSavingThrowRoll(entering, Ability.Wisdom, DC, advantage, feedback);
      bool saveFailed = totalSave < DC;

      SpellUtils.SendSavingThrowFeedbackMessage(intimidator, entering, feedback, advantage, DC, totalSave, saveFailed, Ability.Wisdom);

      if (saveFailed)
        NWScript.AssignCommand(intimidator, () => entering.ApplyEffect(EffectDuration.Temporary, Effroi, NwTimeSpan.FromRounds(1)));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onIntervalAngeDeLaVengeance(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature intimidator)
        return ScriptHandleResult.Handled;

      foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        if (target == intimidator || !target.IsReactionTypeHostile(intimidator) || IsFrightImmune(target, intimidator)
        || target.ActiveEffects.Any(e => e.Tag == FrightenedEffectTag))
          continue;

        SpellConfig.SavingThrowFeedback feedback = new();
        int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(intimidator) + intimidator.GetAbilityModifier(Ability.Charisma);
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Wisdom);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, DC, advantage, feedback);
        bool saveFailed = totalSave < DC;

        SpellUtils.SendSavingThrowFeedbackMessage(intimidator, target, feedback, advantage, DC, totalSave, saveFailed, Ability.Wisdom);

        if (saveFailed)
          NWScript.AssignCommand(intimidator, () => target.ApplyEffect(EffectDuration.Temporary, Effroi, NwTimeSpan.FromRounds(1)));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
