using System;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static class CreatureUtils
  {
    public static void OnMobPerception(CreatureEvents.OnPerception onPerception)
    {
      if (!onPerception.Creature.IsEnemy(onPerception.PerceivedCreature) || onPerception.Creature.IsInCombat)
        return;

      switch (onPerception.PerceptionEventType)
      {
        case PerceptionEventType.Seen:
        case PerceptionEventType.Heard:

          foreach (SpecialAbility ability in onPerception.Creature.SpecialAbilities)
            if (SpellUtils.IsSpellBuff(ability.Spell))
              _ = onPerception.Creature.ActionCastSpellAt(ability.Spell, onPerception.Creature, MetaMagic.Extend, true, 0, ProjectilePathType.Default, true);

          break;
      }
    }
    public static async void OnMobDeathResetSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint spawnPoint = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      await NwTask.Delay(TimeSpan.FromMinutes(10));
      spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
    }
  }
}
