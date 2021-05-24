﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using static NWN.Systems.PlayerSystem;
using Effect = NWN.API.Effect;

namespace NWN.Systems.Arena
{
  public static class ScriptHandlers
  {
    public static void HandlePullRopeChainUse(PlaceableEvents.OnLeftClick onUsed)
    {
      NwPlayer oPC = onUsed.ClickedBy;
      
      if (Players.TryGetValue(oPC.LoginCreature, out Player player))
      {
        if (player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.GetLocalVariable<int>("_IS_PVE_ARENA_CREATURE").HasValue))
          ArenaMenu.DrawRunAwayPage(player);
        else
          ArenaMenu.DrawNextFightPage(player);
      }
    }
    public static async void HandleFight(Player player)
    {
      NwArea oArena = player.oid.LoginCreature.Area;
      NwWaypoint oWaypoint = oArena.FindObjectsOfTypeInArea<NwWaypoint>().Where(w => w.Tag == Config.PVE_ARENA_MONSTER_WAYPOINT_TAG).FirstOrDefault();
      var roundCreatures = Utils.GetCreaturesForRound(player.pveArena.currentRound, player.pveArena.currentDifficulty);

      if (player.pveArena.currentRound == 4 || player.pveArena.currentRound == 8)
      {
        oArena.StopBackgroundMusic();
        oArena.MusicBackgroundDayTrack = 182;
        oArena.MusicBackgroundNightTrack = 182;
        oArena.PlayBackgroundMusic();
      }
      else
      {
        oArena.StopBackgroundMusic();
        oArena.MusicBackgroundDayTrack = 181;
        oArena.MusicBackgroundNightTrack = 181;
        oArena.PlayBackgroundMusic();
      }

      foreach (var creatureResref in roundCreatures.resrefs)
      {
        NwCreature creature = NwCreature.Create(creatureResref, oWaypoint.Location, true);
        creature.GetLocalVariable<int>("_IS_PVE_ARENA_CREATURE").Value = 1;
        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster2));
        creature.ChangeToStandardFaction(StandardFaction.Hostile);
        HandleSpecialBehaviour(creature);
      }

      CancellationTokenSource tokenSource = new CancellationTokenSource();
      Task waitPlayerLeavesArena = NwTask.WaitUntil(() => player.oid.LoginCreature.Location.Area == null, tokenSource.Token);
      Task waitRoundCreaturesDead = NwTask.WaitUntil(() => !oArena.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.GetLocalVariable<int>("_IS_PVE_ARENA_CREATURE").HasValue), tokenSource.Token);

      await NwTask.WhenAny(waitRoundCreaturesDead, waitPlayerLeavesArena);
      tokenSource.Cancel();

      if (waitPlayerLeavesArena.IsCompletedSuccessfully)
        return;

      oArena.StopBackgroundMusic();
      oArena.MusicBackgroundDayTrack = 183;
      oArena.MusicBackgroundNightTrack = 183;
      oArena.PlayBackgroundMusic();

      Log.Info($"{player.oid.LoginCreature.Name} just finished round {player.pveArena.currentRound} - Difficulty {player.pveArena.currentDifficulty}");
      player.pveArena.currentPoints += GetRoundPoints(player);
      player.pveArena.currentRound += 1;

      if (player.pveArena.currentRound > Config.roundMax)
      {
        ArenaMenu.HandleStop(player);
        player.oid.SendServerMessage($"Félicitations, vous avez terminé vos combats et remporté {player.pveArena.currentPoints.ToString().ColorString(Color.WHITE)} sur cette tentative pour un total de {player.pveArena.totalPoints.ToString().ColorString(Color.WHITE)}", Color.ROSE);
      }
      else
      {
        Effect paralyze = Effect.CutsceneParalyze();
        paralyze.SubType = EffectSubType.Supernatural;
        paralyze.Tag = "_ARENA_CUTSCENE_PARALYZE_EFFECT";
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, paralyze);
        ArenaMenu.DrawNextFightPage(player);
      }
    }
    private static uint GetRoundPoints(Player player)
    {
      if (player.pveArena.currentRound == 8)
      {
        return (uint)(Math.Pow(Config.arenaMalusDictionary[player.pveArena.currentMalus].basePoints, player.pveArena.currentRound) * (int)player.pveArena.currentDifficulty);
      }
      else
      {
        double result = (Math.Pow(Config.arenaMalusDictionary[player.pveArena.currentMalus].basePoints, player.pveArena.currentRound) - Math.Pow(Config.arenaMalusDictionary[player.pveArena.currentMalus].basePoints, player.pveArena.currentRound - 1)) * (int)player.pveArena.currentDifficulty;
        if (result < 0)
          result = 1;

        return (uint)result;
      }
    }
    private static void HandleSpecialBehaviour(NwCreature oCreature)
    {
      switch(oCreature.Tag)
      {
        case "dog_sewer_starve":
          oCreature.OnCreatureDamage += HandleDogAttack;
          break;
        case "rat_meca":

          oCreature.GetLocalVariable<int>("_IS_GNOME_MECH").Value = 1;
          ApplyGnomeMechAoE(oCreature);
          oCreature.OnCreatureDamage += AttackSystem.HandleDamageEvent;
          
        break;
        case "bat_sewer":
          oCreature.OnCreatureDamage += HandleBatAttack;
          break;
        case "crab_meca":
          oCreature.OnCreatureDamage += HandleCrabAttack;
          oCreature.OnDamaged += HandleCrabDamaged;
          oCreature.GetLocalVariable<int>("_IS_GNOME_MECH").Value = 1;
          break;
        case "pingu_meca":
          oCreature.GetLocalVariable<int>("_IS_GNOME_MECH").Value = 1;
          break;
        case "dog_meca_defect":
          oCreature.GetLocalVariable<int>("_IS_GNOME_MECH").Value = 1;
          ApplyGnomeMechAoE(oCreature);
          oCreature.OnCreatureDamage += HandleDogAttack;
          oCreature.OnCreatureDamage += AttackSystem.HandleDamageEvent;
          break;
        case "cutter_meca":
          oCreature.GetLocalVariable<int>("_IS_GNOME_MECH").Value = 1;
          ApplyGnomeMechAoE(oCreature);
          break;
      }
    }
    private static void HandleDogAttack(OnCreatureDamage onAttack)
    {
      if (!(onAttack.Target is NwCreature) || CreaturePlugin.GetFlatFooted(onAttack.Target) == 0)
        return;

      NwCreature oTarget = (NwCreature)onAttack.Target;

      if (oTarget.DoSkillCheck(Skill.Discipline, onAttack.DamageData.Base))
        return;

      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.Knockdown(), NwTimeSpan.FromRounds(1));
    }
    private static void HandleBatAttack(OnCreatureDamage onAttack)
    {
      onAttack.DamagedBy.ApplyEffect(EffectDuration.Instant, Effect.Heal(onAttack.DamageData.Base));
      onAttack.DamagedBy.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
    }
    private static void HandleCrabAttack(OnCreatureDamage onAttack)
    {
      if (onAttack.Target.RollSavingThrow(SavingThrow.Will, 12, SavingThrowType.Cold, onAttack.DamagedBy) != SavingThrowResult.Failure)
        return;

      onAttack.Target.ApplyEffect(EffectDuration.Temporary, Effect.Slow(), NwTimeSpan.FromRounds(3));
      onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
    }
    private static void HandleCrabDamaged(CreatureEvents.OnDamaged onDamage)
    {
      if(onDamage.Creature.GetLocalVariable<int>("_NB_TIMES_ATTACKED").Value > 2)
      {
        onDamage.Creature.ApplyEffect(EffectDuration.Instant, Effect.Death());
        onDamage.Creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfElectricExplosion));

        foreach (NwCreature creature in onDamage.Creature.Area.FindObjectsOfTypeInArea<NwCreature>())
        {
          if (creature.GetLocalVariable<int>("_IS_GNOME_MECH").HasNothing
            && creature.RollSavingThrow(SavingThrow.Reflex, 10, SavingThrowType.Electricity, onDamage.Creature) != SavingThrowResult.Failure)
            continue;

          creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(NWN.Utils.random, 4, 1), DamageType.Electrical));
          creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitElectrical));
        }
      }
    }
    private static void ApplyGnomeMechAoE(NwCreature oCreature)
    {
      Effect elecAoE = Effect.AreaOfEffect(25, "mechaura_enter", "mechaura_hb");
      elecAoE.Creator = oCreature;
      elecAoE.Tag = "mechaura_aoe";
      elecAoE.SubType = EffectSubType.Supernatural;
      oCreature.ApplyEffect(EffectDuration.Permanent, elecAoE);
    }
  }
}
