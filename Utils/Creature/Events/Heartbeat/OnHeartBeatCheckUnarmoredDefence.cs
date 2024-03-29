﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnHeartBeatCheckUnarmoredDefence(CreatureEvents.OnHeartbeat onHB)
    {
      NwItem armor = onHB.Creature.GetItemInSlot(InventorySlot.Chest);

      if (armor is null || armor.BaseACValue < 1)
      {
        if (onHB.Creature.GetAbilityModifier(Ability.Constitution) > 0 && !onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.UnarmoredDefenceEffectTag))
          onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetUnarmoredDefenseEffect(onHB.Creature.GetAbilityModifier(Ability.Constitution)));
      }
      else
      {
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckUnarmoredDefence;
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.UnarmoredDefenceEffectTag);
      }
    }
  }
}
