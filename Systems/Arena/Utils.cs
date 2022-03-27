using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using static NWN.Systems.Arena.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class Utils
  {
    public static int GetGoldEntryCost(Difficulty difficulty)
    {
      switch(difficulty)
      {
        default: return 0;

        case Difficulty.Level1: return 50;
        case Difficulty.Level2: return 500;
        case Difficulty.Level3: return 1000;
        case Difficulty.Level4: return 2000;
        case Difficulty.Level5: return 5000;
      }
    }
    public static async void HandleArenaDeath(ModuleEvents.OnPlayerDeath onPlayerDeath)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(3));

      onPlayerDeath.DeadPlayer.LoginCreature.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.SmootherStep, Duration = TimeSpan.FromSeconds(7), PauseWithGame = true }, 
        transform => { transform.Rotation = new Vector3(360, 360, 360); });

      onPlayerDeath.DeadPlayer.LoginCreature.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseOut, Duration = TimeSpan.FromSeconds(4), PauseWithGame = true },
        transform => { transform.Rotation = new Vector3(0, 0, 3); });

      await NwTask.Delay(TimeSpan.FromSeconds(5));

      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonEpicUndead));
      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHarm));
      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComChunkRedLarge));

      await NwTask.Delay(TimeSpan.FromSeconds(2));
      onPlayerDeath.DeadPlayer.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;

      await NwTask.WaitUntil(() => onPlayerDeath.DeadPlayer.LoginCreature.Location.Area != null);

      NWN.Utils.ResetVisualTransform(onPlayerDeath.DeadPlayer.LoginCreature);

      await NwTask.Delay(TimeSpan.FromSeconds(3));

      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRaiseDead));
      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Resurrection());

      ChatSystem.chatService.SendMessage(ChatChannel.PlayerTalk, "Pour obtenir votre amulette de concentration de l'arcane, il vous faut vous enregistrer auprès du juge.", NwObject.FindObjectsWithTag<NwCreature>("pve_arena_host").FirstOrDefault(), onPlayerDeath.DeadPlayer);
    }
    public struct RoundCreatures
    {
      public string[] resrefs;
      public uint points;

      public RoundCreatures(string[] resrefs, uint points)
      {
        this.resrefs = resrefs;
        this.points = points;
      }
    }

    public static RoundCreatures GetCreaturesForRound(uint round, Difficulty difficulty)
    {
      switch (round)
      {
        default: return GetRandomNormalEncounter(difficulty);

        case 4: return GetRandomEliteEncounter(difficulty);
        case 8: return GetRandomBossEncounter(difficulty);
      }
    }

    public static RoundCreatures GetRandomNormalEncounter(Difficulty difficulty)
    {
      var encounters = GetNormalEncounters(difficulty);
      int rand = NWN.Utils.random.Next(0, encounters.Length);
      return encounters[rand];
    }

    public static RoundCreatures GetRandomEliteEncounter(Difficulty difficulty)
    {
      var encounters = GetEliteEncounters(difficulty);

      return encounters[NWN.Utils.random.Next(0, encounters.Length)];
    }

    public static RoundCreatures GetRandomBossEncounter(Difficulty difficulty)
    {
      var encounters = GetBossEncounters(difficulty);

      return encounters[NWN.Utils.random.Next(0, encounters.Length)];
    }
    public static async void RandomizeMalusSelection(Player player, SpellSystem spellSystem)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task malusSelected = NwTask.WaitUntil(() => player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARENA_MALUS_APPLIED").HasValue, tokenSource.Token);
      Task waitingForSelection = NwTask.Delay(TimeSpan.FromSeconds(0.2), tokenSource.Token);

      await NwTask.WhenAny(malusSelected, waitingForSelection);
      tokenSource.Cancel();

      if (malusSelected.IsCompletedSuccessfully)
      {
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARENA_MALUS_APPLIED").Delete();
        return;
      }

      int random = NwRandom.Roll(NWN.Utils.random, 20);

      player.menu.choices.Clear();

      player.menu.choices.Add((
        arenaMalusDictionary[(uint)random].name,
        () => ApplyArenaMalus(player, (uint)random, spellSystem)
      ));

      foreach (string malus in player.pveArena.currentMalusList)
        player.menu.choices.Add((
        malus,
        () => ApplyArenaMalus(player, (uint)random, spellSystem)
      ));

      player.menu.DrawText();
      RandomizeMalusSelection(player, spellSystem);
    }
    public static void ApplyArenaMalus(Player player, uint malus, SpellSystem spellSystem)
    {
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARENA_MALUS_APPLIED").Value = 1;
      player.pveArena.currentMalus = malus;
      player.menu.Close();

      foreach (Effect paralysis in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        player.oid.LoginCreature.RemoveEffect(paralysis);

      if (arenaMalusDictionary.TryGetValue(malus, out ArenaMalus arenaMalus))
      {
        try
        {
          player.pveArena.currentMalusList.Add(arenaMalus.name);
          arenaMalus.applyMalus.Invoke(player);
        }
        catch (Exception e)
        {
          NWN.Utils.LogMessageToDMs(e.Message);
        }
      }

      player.oid.LoginCreature.Location = player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(w => w.Tag == PVE_ARENA_WAYPOINT_TAG).Location; ;
      ScriptHandlers.HandleFight(player, spellSystem);
    }
    public static void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;
      ((NwCreature)onSpellCast.Caster).ControllingPlayer.SendServerMessage("Le contrat de spectateur vous interdit de lancer des sorts à l'intérieur de l'arène.", ColorConstants.Red);
    }
    public static async void RemoveArenaMalus(Player player, string malus, string message)
    {
      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      foreach (Effect arenaMalus in player.oid.LoginCreature.ActiveEffects.Where(f => f.Tag == malus))
        player.oid.LoginCreature.RemoveEffect(arenaMalus);

      player.oid.SendServerMessage(message, ColorConstants.Orange);
    }
    public static void ResetPlayerLocation(Player player)
    {
      Location arenaStartLoc = NwObject.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;

      SqLiteUtils.UpdateQuery("playerCharacters",
        new List<string[]>() { new string[] { "areaTag", arenaStartLoc.Area.Tag }, new string[] { "position", arenaStartLoc.Position.ToString() } },
        new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });
    }
  }
}
