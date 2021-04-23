using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
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

    public static void StopCurrentRun(Player player)
    {
      player.pveArena.totalPoints += player.pveArena.currentPoints;
      player.pveArena.currentPoints = 0;
      player.pveArena.currentRound = 1;
    }
    public static void CancelCurrentRun(Player player)
    {
      player.pveArena.currentPoints = 0;
      player.pveArena.currentRound = 1;
    }

    /*public static bool GetIsRoundInProgress(Player player)
    {
      return player.oid.Area.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.Tag == PVE_ARENA_CREATURE_TAG);
    }

    public static Action<Player> CheckRoundEnded = TimingUtils.Debounce((Player player) =>
    {
      if (!GetIsRoundInProgress(player))
      {
        player.pveArena.currentPoints += player.pveArena.potentialPoints;
        player.pveArena.currentRound += 1;
      }
    }, 0.5f);*/

    public static void HandlePlayerDied(object sender, Player.DeathEventArgs e)
    {
      e.player.OnDeath -= HandlePlayerDied;
      e.player.pveArena.currentPoints = 0;
      e.player.pveArena.currentRound = 1;

      AreaSystem.AreaDestroyer(e.player.oid.Area);
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

      return encounters[NWN.Utils.random.Next(0, encounters.Length - 1)];
    }

    public static RoundCreatures GetRandomEliteEncounter(Difficulty difficulty)
    {
      var encounters = GetEliteEncounters(difficulty);

      return encounters[NWN.Utils.random.Next(0, encounters.Length - 1)];
    }

    public static RoundCreatures GetRandomBossEncounter(Difficulty difficulty)
    {
      var encounters = GetBossEncounters(difficulty);

      return encounters[NWN.Utils.random.Next(0, encounters.Length - 1)];
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

      player.menu.DrawText();

      RandomizeMalusSelection(player);
    }
    private static void ApplyArenaMalus(Player player, uint malus)
    {
      player.oid.SendServerMessage($"Malus appliqué : {malus}");
      player.oid.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").Value = 1;
      player.pveArena.currentMalus = malus;
      player.menu.Close();

      // TODO : appliquer le malus

      Effect paralysis = player.oid.ActiveEffects.FirstOrDefault(e => e.Tag == "_ARENA_CUTSCENE_PARALYZE_EFFECT");
      player.oid.RemoveEffect(paralysis);

      ScriptHandlers.HandleFight(player);
    }
  }
}
