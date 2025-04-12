using Anvil.API.Events;
using Anvil.API;
using System;
using System.Numerics;
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
          if(!onStealth.Creature.KnowsFeat((Feat)CustomSkill.Traqueur4))
          {
            foreach (NwCreature enemy in onStealth.Creature.GetNearestCreatures(CreatureTypeFilter.Alive(true), CreatureTypeFilter.Reputation(ReputationType.Enemy),
              CreatureTypeFilter.Perception(PerceptionType.Seen)))
            {
              if (enemy.DistanceSquared(onStealth.Creature) > 600)
                break;

              if (enemy.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 25, true).Any(c => c == onStealth.Creature))
              {
                double angle = AngleBetween(enemy.Position, onStealth.Creature.Position);
                angle = angle < 0 ? 360 + angle : angle;

                float visionMax = enemy.Rotation + 67 > 360 ? (enemy.Rotation + 67) - 360 : enemy.Rotation + 67;
                float visionMin = enemy.Rotation - 67 < 0 ? 360 + (enemy.Rotation - 67) : enemy.Rotation - 67;

                //StringUtils.DisplayStringToAllPlayersNearTarget(enemy, $"facing {enemy.Rotation}", ColorConstants.Orange, true, true);
                //StringUtils.DisplayStringToAllPlayersNearTarget(enemy, $"vision min {visionMin}", ColorConstants.Orange, true, true);
                //StringUtils.DisplayStringToAllPlayersNearTarget(enemy, $"vision max {visionMax}", ColorConstants.Orange, true, true);
                //StringUtils.DisplayStringToAllPlayersNearTarget(enemy, $"angle {angle}", ColorConstants.Orange, true, true);

                if (visionMax > visionMin)
                {
                  if (angle > visionMin && angle < visionMax)
                  {
                    onStealth.EnterOverride = StealthModeOverride.PreventEnter;
                    onStealth.Creature?.LoginPlayer.SendServerMessage($"{enemy.Name.ColorString(ColorConstants.Cyan)} repère votre tentative de dissimulation", ColorConstants.Orange);
                    onStealth.Creature.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Delete();
                    return;
                  }
                }
                else if ((angle > visionMin) || (angle < visionMax))
                {
                  onStealth.EnterOverride = StealthModeOverride.PreventEnter;
                  onStealth.Creature?.LoginPlayer.SendServerMessage($"{enemy.Name.ColorString(ColorConstants.Cyan)} repère votre tentative de dissimulation", ColorConstants.Orange);
                  onStealth.Creature.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Delete();
                  return;
                }
              }
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
    private static double AngleBetween(Vector3 vector1, Vector3 vector2)
    {
      return Math.Atan2(vector2.Y - vector1.Y, vector2.X - vector1.X) * (180 / Math.PI);
    }
    private static void HandleDetectionRoutine(NwCreature spotter)
    {
      foreach (var spotted in spotter.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 25, true))
      {
        if (spotted == spotter)
          continue;

        double angle = AngleBetween(spotter.Position, spotted.Position);
        angle = angle < 0 ? 360 - angle : angle;

        float visionMax = spotter.Rotation + 67 > 360 ? (spotter.Rotation + 67) - 360 : spotter.Rotation + 67;
        float visionMin = spotter.Rotation - 67 < 0 ? 360 + (spotter.Rotation - 67) : spotter.Rotation - 67;

        if (angle > visionMin && angle < visionMax)
        {
          spotted.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.DurGhostlyVisageNoSound), NwTimeSpan.FromRounds(1));
        }  
      }
    }
  }
}
