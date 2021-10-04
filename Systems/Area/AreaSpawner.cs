using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static async void CreateSpawnChecker(NwArea area)
    {
      var spawnScheduler = ModuleSystem.scheduler.ScheduleRepeating(() =>
      {
        Log.Info("repeating checker on");

        foreach (NwWaypoint spawnPoint in area.FindObjectsOfTypeInArea<NwWaypoint>().Where(wp => wp.Tag == "creature_spawn" && wp.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").HasNothing))
        {
          Log.Info("1");
          if(NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == area && p.ControlledCreature.DistanceSquared(spawnPoint) < 2026))
          {
            Log.Info("2");
            spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Value = true;
            Log.Info("3");
            /*Task waitLoopEnd = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));*/

              NwCreature creature = NwCreature.Deserialize(spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value.ToByteArray());
              if (creature != null)
              {
                Log.Info($"spawned : {creature.Name}");
                creature.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneParalyze());
                creature.Location = spawnPoint.Location;
                creature.OnHeartbeat += CheckIfNoPlayerAround;
                creature.OnDeath += OnMobDeathResetSpawn;
                creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value = spawnPoint;
                HandleSpawnSpecificBehaviour(creature);
              }
            //});
            Log.Info("4");
          }
        }

        Log.Info("repeating checker off");
      }
        , TimeSpan.FromSeconds(1));

      await NwTask.WaitUntil(() => !NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == area));
      spawnScheduler.Dispose();
    }

    private static void CheckIfNoPlayerAround(CreatureEvents.OnHeartbeat onHB)
    {
      Log.Info("HB check player around on");

      if (NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == onHB.Creature.Area && p.ControlledCreature.DistanceSquared(onHB.Creature) < 2026))
      {
        Log.Info("HB check player around off : player found");
        return;
      }

      onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
      onHB.Creature.OnDeath -= LootSystem.HandleLoot;
      onHB.Creature.OnDeath -= OnMobDeathResetSpawn;
      onHB.Creature.Destroy();
      Log.Info($"Destroyed : {onHB.Creature.Name}");

      Log.Info("HB check player around off : no player found");
    }

    private static void OnMobDeathResetSpawn(CreatureEvents.OnDeath onDeath)
    {
      Log.Info("mob dead, resetting on");
      NwWaypoint spawnPoint = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      ModuleSystem.scheduler.Schedule(() => { spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete(); }
      , TimeSpan.FromMinutes(10));

      Log.Info("mob dead, resetting off");
    }

    [ScriptHandler("mobReset_on")]
    private void ApplyResetMobRegen(CallInfo callInfo)
    {
      Log.Info("mob reset on");
      NwCreature creature = (NwCreature)callInfo.ObjectSelf;

      if (!creature.IsValid)
        return;

      Log.Info($"reset regen active on : {creature.Name}");

      creature.HP += (int)(creature.MaxHP * 0.2) + 1;

      if (creature.HP > creature.MaxHP)
        creature.HP = creature.MaxHP;

      //Log.Info($"creature distance from reset : {creature.Distance(creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value)}");

      if (creature.DistanceSquared(creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("_SPAWN").Value) < 1)
      {
        //Log.Info($"{creature.Name} is on reset position !");
        creature.AiLevel = AiLevel.Default;
        foreach (Effect eff in creature.ActiveEffects.Where(e => e.Tag == "mob_reset_regen"))
          creature.RemoveEffect(eff);
      }

      Log.Info("mob reset off");
    }
  }
}
