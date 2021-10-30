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
        foreach (NwWaypoint spawnPoint in area.FindObjectsOfTypeInArea<NwWaypoint>().Where(wp => wp.Tag == "creature_spawn" && wp.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").HasNothing))
        {
          if (NwModule.Instance.Players.Any(p => p.ControlledCreature?.Area == area && p.ControlledCreature.DistanceSquared(spawnPoint) < 2026))
          {
            /*spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Value = true;

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
            }*/
          }
        }
      }
        , TimeSpan.FromSeconds(1));

      await NwTask.WaitUntil(() => !NwModule.Instance.Players.Any(p => p.ControlledCreature?.Area == area));
      spawnScheduler.Dispose();
    }

    private static void CheckIfNoPlayerAround(CreatureEvents.OnHeartbeat onHB)
    {
      if (NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == onHB.Creature.Area && p.ControlledCreature.DistanceSquared(onHB.Creature) < 2026))
        return;

      onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
      onHB.Creature.OnDeath -= LootSystem.HandleLoot;
      onHB.Creature.OnDeath -= OnMobDeathResetSpawn;
      onHB.Creature.Destroy();
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
      NwCreature creature = (NwCreature)callInfo.ObjectSelf;

      if (!creature.IsValid)
        return;;

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
    }
  }
}
