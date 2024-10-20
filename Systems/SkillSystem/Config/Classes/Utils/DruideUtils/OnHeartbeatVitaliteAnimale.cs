using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void OnHeartbeatVitaliteAnimale(CreatureEvents.OnHeartbeat onHB)
    {
      if (onHB.Creature.HP > 0 && onHB.Creature.HP < 60)
      {
        onHB.Creature.ApplyEffect(EffectDuration.Instant, Effect.Heal(NwRandom.Roll(Utils.random, 8)));
        onHB.Creature.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(35), NwTimeSpan.FromRounds(1));
      
      }
    }
  }
}
