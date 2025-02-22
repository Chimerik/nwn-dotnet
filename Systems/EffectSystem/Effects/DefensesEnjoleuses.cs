using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DefensesEnjoleusesEffectTag = "_DEFENSES_ENJOLEUSES_EFFECT";
    private static ScriptCallbackHandle onRemoveDefensesEnjoleusesCallback;
    public static Effect DefensesEnjoleuses
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.DefensesEnjoleuses), Effect.RunAction(onRemovedHandle: onRemoveDefensesEnjoleusesCallback));
        eff.Tag = DefensesEnjoleusesEffectTag;
        eff.SubType = Anvil.API.EffectSubType.Unyielding;

        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveDefensesEnjoleuses(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, Cooldown(caster, 6, CustomSkill.DefensesEnjoleuses), NwTimeSpan.FromRounds(10)));
      }

      return ScriptHandleResult.Handled;
    }
  }
}



