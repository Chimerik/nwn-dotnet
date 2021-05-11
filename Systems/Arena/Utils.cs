using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
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

      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, 360.0f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 7f);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Y, 360.0f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_QUADRATIC, 7f);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 360.0f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_INVERSE_SMOOTHSTEP, 7f);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 2.5f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_EASE_OUT, 4f);

      await NwTask.Delay(TimeSpan.FromSeconds(5));

      onPlayerDeath.DeadPlayer.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.FnfSummonEpicUndead));
      onPlayerDeath.DeadPlayer.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ImpHarm));
      onPlayerDeath.DeadPlayer.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ComChunkRedLarge));

      await NwTask.Delay(TimeSpan.FromSeconds(2));
      onPlayerDeath.DeadPlayer.Location = NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;

      await NwTask.WaitUntil(() => onPlayerDeath.DeadPlayer.Location.Area != null);

      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, 0);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Y, 0);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 0);
      NWScript.SetObjectVisualTransform(onPlayerDeath.DeadPlayer, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 0);

      await NwTask.Delay(TimeSpan.FromSeconds(3));

      onPlayerDeath.DeadPlayer.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ImpRaiseDead));
      onPlayerDeath.DeadPlayer.ApplyEffect(EffectDuration.Instant, API.Effect.Resurrection());

      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Hé ben. Ils vous ont pas loupé là-dedans. On y retourne pour leur montrer ?",
        NwModule.FindObjectsWithTag<NwCreature>("pve_arena_host").FirstOrDefault(), onPlayerDeath.DeadPlayer);
    }
    public static void OnExitArena(AreaEvents.OnExit onExit)
    {
      if (!Players.TryGetValue(onExit.ExitingObject, out Player player))
        return;

      if (player.pveArena.currentRound == 0)
      {
        player.oid.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
        player.oid.OnSpellCast -= Utils.NoMagicMalus;
        return;
      }
        

      player.oid.OnPlayerDeath -= HandleArenaDeath; 
      player.oid.OnPlayerDeath += HandlePlayerDeath;

      player.pveArena.currentPoints = 0;
      player.pveArena.currentRound = 0;

      player.pveArena.currentMalusList.Clear();

      foreach (API.Effect paralysis in player.oid.ActiveEffects.Where(e => e.Tag == "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        player.oid.RemoveEffect(paralysis);

      foreach(NwPlayer spectator in NwModule.Instance.Players.Where(p => p.Area == onExit.Area))
      {
        spectator.SendServerMessage($"La tentative de {player.oid.Name} s'achève. Vous êtes reconduit à la salle principale.");
        spectator.Location = NwModule.FindObjectsWithTag<NwWaypoint>(Config.PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;
      }

      AreaSystem.AreaDestroyer(onExit.Area);

      if(onExit.IsDisconnectingPlayer)
      {
        API.Location arenaStartLoc = NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;

        var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"UPDATE playerCharacters SET areaTag = @areaTag, position = @position WHERE characterId = @characterId");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@areaTag", arenaStartLoc.Area.Tag);
        NWScript.SqlBindVector(query, "@position", arenaStartLoc.Position);
        NWScript.SqlStep(query);
      }
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

      Task malusSelected = NwTask.WaitUntil(() => player.oid.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").HasValue, tokenSource.Token);
      Task waitingForSelection = NwTask.Delay(TimeSpan.FromSeconds(0.2), tokenSource.Token);

      await NwTask.WhenAny(malusSelected, waitingForSelection);
      tokenSource.Cancel();

      if (malusSelected.IsCompletedSuccessfully)
      {
        player.oid.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").Delete();
        return;
      }

      int random = NwRandom.Roll(NWN.Utils.random, 20);

      player.menu.choices.Clear();

      player.menu.choices.Add((
        Config.arenaMalusDictionary[(uint)random].name,
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
      player.oid.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").Value = 1;
      player.pveArena.currentMalus = malus;
      player.menu.Close();

      foreach (API.Effect paralysis in player.oid.ActiveEffects.Where(e => e.Tag == "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        player.oid.RemoveEffect(paralysis);

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

      player.oid.JumpToObject(player.oid.Area.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(w => w.Tag == PVE_ARENA_WAYPOINT_TAG));
      ScriptHandlers.HandleFight(player);
    }
    public static void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;
      ((NwPlayer)onSpellCast.Caster).SendServerMessage("Le contrat de spectateur vous interdit de lancer des sorts à l'intérieur de l'arène.", Color.RED);
    }
  }
}
