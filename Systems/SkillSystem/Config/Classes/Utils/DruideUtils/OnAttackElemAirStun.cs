using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void OnAttackElemAirStun(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target || target.IsImmuneTo(ImmunityType.Stun))
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (CreatureUtils.GetSavingThrow(onAttack.Attacker, target, Ability.Constitution, 13) == SavingThrowResult.Failure)
          {
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpStun));
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Stunned(), Effect.VisualEffect(VfxType.DurMindAffectingDisabled)), NwTimeSpan.FromRounds(2)));
          }

          break;
      }
    }
  }
}
