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
    
    public static Effect AngeDeLaVengeance(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraNegativeEnergy, fScale:2),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterAngeDeLaVengeanceCallback, heartbeatHandle: onIntervalAngeDeLaVengeanceCallback));
      eff.Tag = PresenceIntimidanteAuraEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      return eff;
    }
    private static ScriptHandleResult onEnterAngeDeLaVengeance(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature intimidator || intimidator.HP < 1 || entering == intimidator
        || !entering.IsReactionTypeHostile(intimidator) || IsFrightImmune(entering, intimidator))
        return ScriptHandleResult.Handled;

      int saveDC = 8 + NativeUtils.GetCreatureProficiencyBonus(intimidator) + intimidator.GetAbilityModifier(Ability.Charisma);

      if (CreatureUtils.GetSavingThrow(intimidator, entering, Ability.Wisdom, saveDC) == SavingThrowResult.Failure)
        NWScript.AssignCommand(intimidator, () => entering.ApplyEffect(EffectDuration.Temporary, Effroi(entering), NwTimeSpan.FromRounds(1)));

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

        int saveDC = 8 + NativeUtils.GetCreatureProficiencyBonus(intimidator) + intimidator.GetAbilityModifier(Ability.Charisma);

        if (CreatureUtils.GetSavingThrow(intimidator, target, Ability.Wisdom, saveDC) == SavingThrowResult.Failure)
          NWScript.AssignCommand(intimidator, () => target.ApplyEffect(EffectDuration.Temporary, Effroi(target), NwTimeSpan.FromRounds(1)));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
