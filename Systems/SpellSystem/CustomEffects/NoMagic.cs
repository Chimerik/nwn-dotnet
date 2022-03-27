using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  /*static class NoMagic
  {
    public static ScriptHandleResult ApplyEffectToTarget(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (!(eventData.EffectTarget is NwCreature oTarget))
        return ScriptHandleResult.Handled;

      oTarget.OnSpellCast -= SpellSystem.HandleBeforeSpellCast;
      oTarget.OnSpellCast -= NoMagicMalus;
      oTarget.OnSpellCast += NoMagicMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      return ScriptHandleResult.Handled;
    }
    public static ScriptHandleResult RemoveEffectFromTarget(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (!(eventData.EffectTarget is NwCreature oTarget))
        return ScriptHandleResult.Handled;

      oTarget.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      oTarget.OnSpellCast -= NoMagicMalus;

      return ScriptHandleResult.Handled;
    }
    private static void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;

      if (onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
        oPC.ControllingPlayer.SendServerMessage("L'interdiction d'usage de sorts est en vigueur.", ColorConstants.Red);
    }
  }*/
}
