using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
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

      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer.LoginCreature, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, 360.0f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 7f);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer.LoginCreature, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Y, 360.0f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_QUADRATIC, 7f);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer.LoginCreature, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 360.0f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_INVERSE_SMOOTHSTEP, 7f);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer.LoginCreature, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 2.5f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_EASE_OUT, 4f);

      await NwTask.Delay(TimeSpan.FromSeconds(5));

      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonEpicUndead));
      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHarm));
      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComChunkRedLarge));

      await NwTask.Delay(TimeSpan.FromSeconds(2));
      onPlayerDeath.DeadPlayer.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;

      await NwTask.WaitUntil(() => onPlayerDeath.DeadPlayer.LoginCreature.Location.Area != null);

      NWN.Utils.ResetVisualTransform(onPlayerDeath.DeadPlayer.LoginCreature);

      await NwTask.Delay(TimeSpan.FromSeconds(3));

      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ImpRaiseDead));
      onPlayerDeath.DeadPlayer.LoginCreature.ApplyEffect(EffectDuration.Instant, API.Effect.Resurrection());

      ChatSystem.chatService.SendMessage(Services.ChatChannel.PlayerTalk, "Pour obtenir votre amulette de concentration de l'arcane, il vous faut vous enregistrer auprès du juge.", NwObject.FindObjectsWithTag<NwCreature>("pve_arena_host").FirstOrDefault(), onPlayerDeath.DeadPlayer);
    }
    public static void OnExitArena(AreaEvents.OnExit onExit)
    {
      if (!(onExit.ExitingObject is NwCreature creature) || !Players.TryGetValue(onExit.ExitingObject, out Player player))
        return;

      if (creature.IsPlayerControlled) // Cas normal de changement de zone
        if (!Players.TryGetValue(creature.ControllingPlayer.LoginCreature, out player))
          return;
      else // cas de déconnexion du joueur
        ResetPlayerLocation(player);

      if (player.pveArena.currentRound == 0) // S'il s'agit d'un spectateur
      {
        player.oid.LoginCreature.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
        player.oid.LoginCreature.OnSpellCast -= NoMagicMalus;
        return;
      }

      // A partir de là, il s'agit du gladiateur
      player.oid.OnPlayerDeath -= HandleArenaDeath;
      player.oid.OnPlayerDeath += HandlePlayerDeath;

      player.pveArena.currentPoints = 0;
      player.pveArena.currentRound = 0;

      player.pveArena.currentMalusList.Clear();

      foreach (API.Effect paralysis in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        player.oid.LoginCreature.RemoveEffect(paralysis);

      foreach (NwCreature spectator in onExit.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled || p.IsLoginPlayerCharacter))
      {
        spectator.ControllingPlayer.SendServerMessage($"La tentative de {player.oid.LoginCreature.Name} s'achève. Vous êtes reconduit à la salle principale.");
        spectator.Location = NwObject.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;
      }

      AreaSystem.AreaDestroyer(onExit.Area);
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
    public static async void RandomizeMalusSelection(Player player)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task malusSelected = NwTask.WaitUntil(() => player.oid.LoginCreature.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").HasValue, tokenSource.Token);
      Task waitingForSelection = NwTask.Delay(TimeSpan.FromSeconds(0.2), tokenSource.Token);

      await NwTask.WhenAny(malusSelected, waitingForSelection);
      tokenSource.Cancel();

      if (malusSelected.IsCompletedSuccessfully)
      {
        player.oid.LoginCreature.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").Delete();
        return;
      }

      int random = NwRandom.Roll(NWN.Utils.random, 20);

      player.menu.choices.Clear();

      player.menu.choices.Add((
        arenaMalusDictionary[(uint)random].name,
        () => ApplyArenaMalus(player, (uint)random)
      ));

      foreach (string malus in player.pveArena.currentMalusList)
        player.menu.choices.Add((
        malus,
        () => ApplyArenaMalus(player, (uint)random)
      ));

      player.menu.DrawText();
      RandomizeMalusSelection(player);
    }
    public static void ApplyArenaMalus(Player player, uint malus)
    {
      player.oid.LoginCreature.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").Value = 1;
      player.pveArena.currentMalus = malus;
      player.menu.Close();

      foreach (API.Effect paralysis in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
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
      ScriptHandlers.HandleFight(player);
    }
    public static void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;
      ((NwCreature)onSpellCast.Caster).ControllingPlayer.SendServerMessage("Le contrat de spectateur vous interdit de lancer des sorts à l'intérieur de l'arène.", ColorConstants.Red);
    }
    public static async void RemoveArenaMalus(Player player, string malus, string message)
    {
      await NwTask.WaitUntil(() => player.pveArena.currentRound == 0);

      foreach (API.Effect arenaMalus in player.oid.LoginCreature.ActiveEffects.Where(f => f.Tag == malus))
        player.oid.LoginCreature.RemoveEffect(arenaMalus);

      player.oid.SendServerMessage(message, ColorConstants.Orange);
    }
    private static void ResetPlayerLocation(Player player)
    {
      Location arenaStartLoc = NwObject.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;

      SqLiteUtils.UpdateQuery("playerCharacters",
        new List<string[]>() { new string[] { "areaTag", arenaStartLoc.Area.Tag }, new string[] { "position", arenaStartLoc.Position.ToString() } },
        new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });
    }
  }
}
