using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  static class NoSummon
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnAssociateAdd -= NoSummoningSpellMalus;
      oTarget.OnAssociateAdd += NoSummoningSpellMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      foreach (NwCreature summon in oTarget.Henchmen)
      {
        summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
        summon.Destroy();
      }
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnAssociateAdd -= NoSummoningSpellMalus;
    }
    private static void NoSummoningSpellMalus(OnAssociateAdd onAssociateAdd)
    {
      foreach (NwCreature summon in onAssociateAdd.Owner.Henchmen)
      {
        summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
        summon.Destroy();
      }

      if (onAssociateAdd.Owner.IsPlayerControlled)
        onAssociateAdd.Owner.ControllingPlayer.SendServerMessage("L'interdiction d'usage d'invocations est en vigueur.", Color.RED);
    }
  }
}
