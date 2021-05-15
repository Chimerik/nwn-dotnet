using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class NoSummon
  {
    public NoSummon(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
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
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnAssociateAdd -= NoSummoningSpellMalus;
    }
    private void NoSummoningSpellMalus(OnAssociateAdd onAssociateAdd)
    {
      foreach (NwCreature summon in onAssociateAdd.Owner.Henchmen)
      {
        summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
        summon.Destroy();
      }

      ((NwPlayer)onAssociateAdd.Owner).SendServerMessage("L'interdiction d'usage d'invocations est en vigueur.", Color.RED);
    }
  }
}
