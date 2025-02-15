using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void HandlePush(NwCreature attacker, NwGameObject target, int pushDistance)
    {
      if (target is NwCreature targetCreature && targetCreature.Size < CreatureSize.Huge)
      {
        //target.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(50), NwTimeSpan.FromRounds(1));

        float newX = target.Position.X + 4 * ((target.Position.X - attacker.Position.X) / attacker.Distance(target));
        float newY = target.Position.Y + 4 * ((target.Position.Y - attacker.Position.Y) / attacker.Distance(target));

        //ModuleSystem.Log.Info($"target pos : {target.Position.X} - {target.Position.Y} - {target.Position.Z}");
        //ModuleSystem.Log.Info($"new pos : {newX} - {newY} - {target.Position.Z}");

        if (newX != 0 && newY != 0)
        {
          await NwTask.NextFrame();
          var newPos = CreaturePlugin.ComputeSafeLocation(targetCreature, new Vector3(newX, newY, target.Position.Z), 4, 1);

          if (newPos != Vector3.Zero)
          {
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
            target.Position = newPos;
          } 
        }
      }
    }
  }
}
