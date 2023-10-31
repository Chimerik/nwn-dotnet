﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void Dodge(NwCreature caster)
    {
      await NwTask.NextFrame();
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.dodgeEffect, NwTimeSpan.FromRounds(2));
    }
  }
}