﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkParadeEffectTag = "_MONK_PARADE_EFFECT";
    public static readonly CExoString MonkParadeEffectExoTag = MonkParadeEffectTag.ToExoString();
    private static ScriptCallbackHandle onRemoveMonkParadeCallback;
    public static Effect MonkParade
    {
      get
      {
        Effect eff = Effect.RunAction(onRemovedHandle: onRemoveMonkParadeCallback);
        eff.Tag = MonkParadeEffectTag;
        eff.SubType = Anvil.API.EffectSubType.Unyielding;

        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveMonkParade(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, Cooldown(caster, CustomSkill.MonkParade, 6), NwTimeSpan.FromRounds(1)));
      }

      return ScriptHandleResult.Handled;
    }
  }
}



