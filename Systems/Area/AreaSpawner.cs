using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using System;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static async void CreateSpawnChecker(NwArea area)
    {
      var spawnScheduler = ModuleSystem.scheduler.ScheduleRepeating(() =>
      {
        foreach(NwWaypoint spawnPoint in area.FindObjectsOfTypeInArea<NwWaypoint>().Where(wp => wp.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").HasNothing && wp.GetNearestCreatures().FirstOrDefault(p => p.IsPlayerControlled)?.DistanceSquared(wp) < 2026))
        {
          NwCreature creature = NwCreature.Deserialize(spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value.ToByteArray());

          if (creature != null)
          {
            creature.Location = spawnPoint.Location;
            creature.OnHeartbeat += CheckIfNoPlayerAround;
            creature.OnDeath += OnMobDeathResetSpawn;
            creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value = spawnPoint;
            HandleSpawnSpecificBehaviour(creature);
            spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Value = true;
          }
        }
      }
        , TimeSpan.FromSeconds(1));

      await NwTask.WaitUntil(() => !NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == area));
      spawnScheduler.Dispose();
    }

    private static void CheckIfNoPlayerAround(CreatureEvents.OnHeartbeat onHB)
    {
      NwCreature nearestPC = onHB.Creature.GetNearestCreatures().FirstOrDefault(p => p.IsPlayerControlled);
      if (nearestPC is null || onHB.Creature.DistanceSquared(nearestPC) > 2500)
      {
        onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
        onHB.Creature.OnDeath -= LootSystem.HandleLoot;
        onHB.Creature.OnDeath -= OnMobDeathResetSpawn;
        onHB.Creature.Destroy();
      }
    }

    private static void OnMobDeathResetSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint spawnPoint = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      ModuleSystem.scheduler.Schedule(() => { spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete(); }
      , TimeSpan.FromMinutes(10));
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
