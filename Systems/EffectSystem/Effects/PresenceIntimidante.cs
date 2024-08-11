using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PresenceIntimidanteAuraEffectTag = "_PRESENCE_INTIMIDANTE_AURA_EFFECT";
    private static ScriptCallbackHandle onEnterPresenceIntimidanteStyleCallback;
    private static ScriptCallbackHandle onIntervalPresenceIntimidanteCallback;
    
    public static Effect presenceIntimidante
    {
      get
      {
        Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobDragonFear, onEnterHandle: onEnterPresenceIntimidanteStyleCallback, heartbeatHandle: onIntervalPresenceIntimidanteCallback);
        eff.Tag = PresenceIntimidanteAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterPresenceIntimidante(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature intimidator || intimidator.HP < 1 || entering == intimidator
        || !entering.IsReactionTypeHostile(intimidator) || IsFrightImmune(entering, intimidator)
        || entering.GetObjectVariable<LocalVariableInt>($"_PRESENCE_INTIMIDANTE_RESISTED_{intimidator.Name}").HasValue)
        return ScriptHandleResult.Handled;

      int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(intimidator) + intimidator.GetAbilityModifier(Ability.Charisma);

      if (CreatureUtils.GetSavingThrow(intimidator, entering, Ability.Wisdom, DC) == SavingThrowResult.Failure)
        NWScript.AssignCommand(intimidator, () => entering.ApplyEffect(EffectDuration.Temporary, Effroi, NwTimeSpan.FromRounds(1)));
      else
        entering.GetObjectVariable<LocalVariableInt>($"_PRESENCE_INTIMIDANTE_RESISTED_{intimidator.Name}").Value = 1;

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onIntervalPresenceIntimidante(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature intimidator)
        return ScriptHandleResult.Handled;

      foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        if (target == intimidator || !target.IsReactionTypeHostile(intimidator) || IsFrightImmune(target, intimidator)
        || target.GetObjectVariable<LocalVariableInt>($"_PRESENCE_INTIMIDANTE_RESISTED_{intimidator.Name}").HasValue)
          continue;

        int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(intimidator) + intimidator.GetAbilityModifier(Ability.Charisma);

        if (CreatureUtils.GetSavingThrow(intimidator, target, Ability.Wisdom, DC) == SavingThrowResult.Failure)
          NWScript.AssignCommand(intimidator, () => target.ApplyEffect(EffectDuration.Temporary, Effroi, NwTimeSpan.FromRounds(1)));
        else
          target.GetObjectVariable<LocalVariableInt>($"_PRESENCE_INTIMIDANTE_RESISTED_{intimidator.Name}").Value = 1;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
