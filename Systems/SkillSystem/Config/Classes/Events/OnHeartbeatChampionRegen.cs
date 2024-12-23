﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnHeartbeatUltimeSurvivant(CreatureEvents.OnHeartbeat onHB)
    {
      if (onHB.Creature.HP > 0 && onHB.Creature.HP < onHB.Creature.MaxHP / 2)
        onHB.Creature.ApplyEffect(EffectDuration.Instant, Effect.Heal(5 + onHB.Creature.GetAbilityModifier(Ability.Constitution)));
    }
  }
}
