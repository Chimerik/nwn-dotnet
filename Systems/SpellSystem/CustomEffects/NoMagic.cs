using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class NoMagic
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= SpellSystem.HandleBeforeSpellCast;
      oTarget.OnSpellCast -= NoMagicMalus;
      oTarget.OnSpellCast += NoMagicMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      oTarget.OnSpellCast -= NoMagicMalus;
    }
    private static void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;

      if (onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
        oPC.ControllingPlayer.SendServerMessage("L'interdiction d'usage de sorts est en vigueur.", ColorConstants.Red);
    }
  }
}
