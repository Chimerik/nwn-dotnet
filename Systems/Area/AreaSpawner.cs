using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NWN.Core;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static async void CreateSpawnAoE(NwWaypoint spawnPoint)
    {
      Log.Info("create spawn aoe on");
      if (spawnPoint.GetObjectVariable<LocalVariableBool>("_CAN_SPAWN").HasNothing)
        return;

      spawnPoint.GetObjectVariable<LocalVariableBool>("_CAN_SPAWN").Delete();

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      Effect spawnEffect = Effect.AreaOfEffect(198, "enterSpawn");
      spawnEffect.SubType = EffectSubType.Supernatural;
      spawnEffect.Tag = "creature_spawn_aoe";
      spawnPoint.Location.ApplyEffect(EffectDuration.Permanent, spawnEffect);

      NwAreaOfEffect spawnAoE = (NwAreaOfEffect)NwModule.Instance.GetLastCreatedObjects().FirstOrDefault(aoe => aoe is NwAreaOfEffect);
      spawnAoE.GetObjectVariable<LocalVariableString>("creature").Value = spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value;
      spawnAoE.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value = spawnPoint;
      spawnAoE.Tag = "creature_spawn_aoe";

      Log.Info("create spawn aoe player off");
    }

    [ScriptHandler("enterSpawn")]
    private void OnEnterSpawnPoint(CallInfo callInfo)
    {
      Log.Info("enter spawn on");
      if (NWScript.GetEnteringObject().ToNwObject() is NwCreature oEntered && oEntered.IsPlayerControlled)
      {
        Log.Info("1");
        NwAreaOfEffect spawnAOE = ((NwAreaOfEffect)callInfo.ObjectSelf);
        Log.Info("2");
        NwCreature creature = NwCreature.Deserialize(spawnAOE.GetObjectVariable<LocalVariableString>("creature").Value.ToByteArray());
        Log.Info("3");
        if (creature == null)
          return;
        Log.Info($"Spawning : {creature.Name}");
        Log.Info("4");
        creature.Location = spawnAOE.Location;
        Log.Info("5");
        NwWaypoint spawnPoint = spawnAOE.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value;
        spawnPoint.GetObjectVariable<LocalVariableBool>("active").Value = true;
        creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value = spawnPoint;
        Log.Info("6");
        Effect exitMobRangeEffect = Effect.AreaOfEffect(198, "", "", "exitMobRange");
        exitMobRangeEffect.SubType = EffectSubType.Supernatural;
        exitMobRangeEffect.Tag = "creature_exit_range_aoe";
        creature.ApplyEffect(EffectDuration.Permanent, exitMobRangeEffect);
        Log.Info("7");
        NwAreaOfEffect exitMobRangeAoE = (NwAreaOfEffect)NwModule.Instance.GetLastCreatedObjects().FirstOrDefault(aoe => aoe is NwAreaOfEffect);
        exitMobRangeAoE.GetObjectVariable<LocalVariableObject<NwCreature>>("creature").Value = creature;
        exitMobRangeAoE.Tag = "creature_exit_range_aoe";
        Log.Info("8");
        creature.OnDeath += OnMobDeathResetSpawn;
        Log.Info("9");
        HandleSpawnSpecificBehaviour(creature);
        Log.Info("10");
        spawnAOE.Destroy();

        Log.Info("enter spawn pc off");
      }
      else
        Log.Info("enter spawn npc off");
    }

    private static void OnMobDeathResetSpawn(CreatureEvents.OnDeath onDeath)
    {
      Log.Info("mob death reset on");
      NwWaypoint spawnPoint = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value;

      ModuleSystem.scheduler.Schedule(() => 
      { 
        spawnPoint.GetObjectVariable<LocalVariableBool>("_CAN_SPAWN").Value = true;
        if (NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == spawnPoint.Area))
          CreateSpawnAoE(spawnPoint);
      }
      , TimeSpan.FromMinutes(10));

      spawnPoint.GetObjectVariable<LocalVariableBool>("active").Delete();
      if (onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").HasValue)
        onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value.Destroy();

      Log.Info("mob death reset off");
    }

    [ScriptHandler("exitSpawn")]
    private void OnExitSpawnPoint(CallInfo callInfo)
    {
      Log.Info("exit spawn on");
      if (NWScript.GetExitingObject().ToNwObject() is NwCreature oCreature && oCreature == callInfo.ObjectSelf.GetObjectVariable<LocalVariableObject<NwCreature>>("creature").Value)
      {
        Log.Info($"{oCreature.Name} returning to spawn point");
        oCreature.AiLevel = AiLevel.VeryLow;
        oCreature.ClearActionQueue();
        oCreature.ActionForceMoveTo(callInfo.ObjectSelf, true, 0, TimeSpan.FromSeconds(30));
        Effect regen = NWScript.EffectRunScript("", "mobReset_off", "mobReset_on", 1);
        regen.Tag = "mob_reset_regen";
        regen.SubType = EffectSubType.Supernatural;
        oCreature.ApplyEffect(EffectDuration.Permanent, regen);
      }
      Log.Info("exit spawn off");
    }

    [ScriptHandler("mobReset_on")]
    private void ApplyResetMobRegen(CallInfo callInfo)
    {
      Log.Info("mob reset on");
      NwCreature creature = (NwCreature)callInfo.ObjectSelf;

      if (!creature.IsValid || !creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value.IsValid)
        return;

      Log.Info($"reset regen active on : {creature.Name}");

      creature.HP += (int)(creature.MaxHP * 0.2) + 1;

      if (creature.HP > creature.MaxHP)
        creature.HP = creature.MaxHP;

      //Log.Info($"creature distance from reset : {creature.Distance(creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value)}");

      if (creature.Distance(creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value) < 1)
      {
        //Log.Info($"{creature.Name} is on reset position !");
        creature.AiLevel = AiLevel.Default;
        foreach (Effect eff in creature.ActiveEffects.Where(e => e.Tag == "mob_reset_regen"))
          creature.RemoveEffect(eff);
      }

      Log.Info("mob reset off");
    }

    [ScriptHandler("exitMobRange")]
    private void ResetSpawnPoint(CallInfo callInfo)
    {
      Log.Info("exit mob range on");
      if (!(NWScript.GetExitingObject().ToNwObject() is NwCreature { IsPlayerControlled: true }))
      {
        Log.Info("exit mob range npc off");
        return;
      }

      Log.Info("exit mob range pc on");

      NwAreaOfEffect aoe = (NwAreaOfEffect)callInfo.ObjectSelf;
      NwCreature creature = aoe.GetObjectVariable<LocalVariableObject<NwCreature>>("creature").Value;
      NwWaypoint spawnPoint = creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value;
      spawnPoint.GetObjectVariable<LocalVariableBool>("active").Delete();

      creature.OnDeath -= LootSystem.HandleLoot;
      creature.OnDeath -= OnMobDeathResetSpawn;

      if (creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").HasValue)
        creature.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value.Destroy();

      creature.Destroy();
      Log.Info($"Destroyed : {creature.Name}");

      if (!NwModule.Instance.Players.Any(c => c.ControlledCreature.Area == spawnPoint.Area))
        return;

      Effect spawnEffect = Effect.AreaOfEffect(198, "enterSpawn");
      spawnEffect.SubType = EffectSubType.Supernatural;
      spawnEffect.Tag = "creature_spawn_aoe";
      spawnPoint.Location.ApplyEffect(EffectDuration.Permanent, spawnEffect);

      NwAreaOfEffect spawnAoE = (NwAreaOfEffect)NwModule.Instance.GetLastCreatedObjects().FirstOrDefault(aoe => aoe is NwAreaOfEffect);
      spawnAoE.GetObjectVariable<LocalVariableString>("creature").Value = spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value;
      spawnAoE.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value = spawnPoint;
      spawnAoE.Tag = "creature_spawn_aoe";

      Log.Info("exit mob range off");
    }
  }
}
