﻿using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnStealth(OnStealthModeUpdate onStealth)
    {
      if(onStealth.EventType == ToggleModeEventType.Enter)
      {
        if (onStealth.Creature.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").HasNothing)
          onStealth.EnterOverride = StealthModeOverride.PreventEnter;
        else
        {
          foreach(NwCreature enemy in onStealth.Creature.GetNearestCreatures(CreatureTypeFilter.Alive(true), CreatureTypeFilter.Reputation(ReputationType.Enemy),
            CreatureTypeFilter.Perception(PerceptionType.Seen)))
          {
            if (enemy.DistanceSquared(onStealth.Creature) > 600)
              break;

            if(enemy.Location.GetObjectsInShapeByType<NwCreature>(Shape.Cone, 135, true, enemy.Position).Any(c => c == onStealth.Creature))
            {
              onStealth.EnterOverride = StealthModeOverride.PreventEnter;
              onStealth.Creature?.LoginPlayer.SendServerMessage($"{enemy.Name.ColorString(ColorConstants.Cyan)} repère votre tentative de dissimulation", ColorConstants.Orange);
              onStealth.Creature.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Delete();
              return;
            }
          }

          onStealth.EnterOverride = StealthModeOverride.None;
          onStealth.Creature.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Delete();
          onStealth.Creature.OnSpellBroadcast += SpellSystem.OnSpellCastCancelStealth;
        }
      }
      else
        onStealth.Creature.OnSpellBroadcast -= SpellSystem.OnSpellCastCancelStealth;
    }
  }
}
