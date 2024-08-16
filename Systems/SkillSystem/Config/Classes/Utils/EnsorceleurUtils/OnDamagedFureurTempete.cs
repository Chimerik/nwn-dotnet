using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void OnDamagedFureurTempete(CreatureEvents.OnDamaged onDamaged)
    {
      if (NWScript.GetLastDamager(onDamaged.Creature).ToNwObject<NwObject>() is not NwCreature damager)
        return;

      damager.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(50), NwTimeSpan.FromRounds(1));
    }
  }
}
