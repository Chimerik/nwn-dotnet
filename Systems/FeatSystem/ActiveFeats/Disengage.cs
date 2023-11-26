using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void Disengage(NwCreature caster)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(3));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));
    }
  }
}
