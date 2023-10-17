using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static async void SeverArtery(Player player, NwGameObject targetObject, WeaponAttackType attackType)
    {
      double severArteryDuration = 5;
      int duration = GetBleedingModifiedDuration(player, targetObject, attackType, severArteryDuration);

      if (duration < 1)
        return;

      await NwTask.NextFrame();
      //targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Death(false, false));
      targetObject.ApplyEffect(EffectDuration.Temporary, bleeding, TimeSpan.FromSeconds(duration));
    }
  }
}
