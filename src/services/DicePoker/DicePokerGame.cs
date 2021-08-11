using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.Systems;

using Anvil.API;
using System.Threading;
using Utils;
using System.ComponentModel;

namespace DicePoker
{
  public class DicePokerGame
  {
    PlayerSystem.Player playerOne;
    PlayerSystem.Player playerTwo;
    NwPlaceable diceBoard;
    uint bet;
    uint gameState;

    public DicePokerGame(PlayerSystem.Player player, NwPlaceable board)
    {
      diceBoard = board;
      playerOne = player;
      diceBoard.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value = 1;
      bet = 100;
      gameState = 0;
      DrawStartingPage();
      AwaitGameEnd(player);
    }
  private async void AwaitGameEnd(PlayerSystem.Player player)
  {
    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Task gameEnded = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").HasValue, tokenSource.Token);
    Task menuClosed = NwTask.WaitUntil(() => player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CURRENT_MENU_CLOSED").HasValue, tokenSource.Token);

    await NwTask.WhenAny(gameEnded, menuClosed);
    tokenSource.Cancel();

    if (gameEnded.IsCompletedSuccessfully)
      return;

    foreach (var local in playerOne.oid.LoginCreature.LocalVariables.Where(l => l.Name.StartsWith("_DICE_POKER")))
      local.Delete();

    diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").Value = 1;
    diceBoard.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value = 2;
    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CURRENT_MENU_CLOSED").Delete();

    switch (gameState)
    {
      case 0:
        playerOne.menu.Close();
        playerOne.oid.SendServerMessage("Vous annulez la partie !", ColorConstants.Orange);
        break;
      case 1:
        foreach (var local in playerTwo.oid.LoginCreature.LocalVariables.Where(l => l.Name.StartsWith("_DICE_POKER")))
          local.Delete();

        playerOne.menu.Close();
        playerTwo.menu.Close();
        break;
      case 2:
          PlayerSystem.Player winner;

        if (player == playerOne)
          winner = playerTwo;
        else
          winner = playerOne;

        player.oid.SendServerMessage($"Vous annulez la partie. La mise de {bet} est donc reversée à {winner.oid.LoginCreature.Name}");
        winner.oid.SendServerMessage($"{player.oid.LoginCreature.Name} vient d'annuler la partie. La mise de {bet} vous a été reversée.");

        winner.oid.LoginCreature.GiveGold((int)bet);

        foreach (var local in playerTwo.oid.LoginCreature.LocalVariables.Where(l => l.Name.StartsWith("_DICE_POKER")))
          local.Delete();

        playerOne.menu.Close();
        playerTwo.menu.Close();
        break;
      case 3:
        playerOne.oid.SendServerMessage("La partie est terminée !");
        playerTwo.oid.SendServerMessage("La partie est terminée !");
        playerOne.menu.Close();
        playerTwo.menu.Close();
        break;
    }

    Task waitForAnimation = NwTask.Run(async () =>
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));
      diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").Delete();
    });
  }
  private void DrawStartingPage()
  {
    playerOne.menu.Clear();

    playerOne.menu.titleLines = new List<string>() {
        "Bienvenue sur le jeu du poker de dés !",
        $"La mise est actuellement de {bet} pièce(s) d'or.",
        "Que souhaitez-vous faire ?"
      };

    playerOne.menu.choices.Add((
      "Modifier la mise",
      () => WaitBetInput()
    ));

    playerOne.menu.choices.Add((
      "Attendre un adversaire",
      () => WaitOpponent()
    ));

    playerOne.menu.choices.Add((
        "Quitter",
        () => playerOne.menu.Close())
      );

    playerOne.menu.Draw();
  }
  private async void WaitBetInput()
  {
    playerOne.menu.Clear();

    playerOne.menu.titleLines = new List<string>() {
        "Veuillez entrer la mise souhaitée.",
      };

    playerOne.menu.Draw();

    bool awaitedValue = await playerOne.WaitForPlayerInputInt();

    if (awaitedValue)
      SetNewBet();
  }
  private void SetNewBet()
  {
    int playerInput = int.Parse(playerOne.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));
    playerOne.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();

    if (playerInput < 0)
    {
      playerOne.oid.SendServerMessage("La mise ne peut pas être inférieur à 0. Veuilez saisir une nouvelle mise", ColorConstants.Red);
      DrawStartingPage();
      return;
    }

    if (!HandleGold(playerOne))
    {
      playerOne.oid.SendServerMessage("Vous n'avez pas assez d'or pour honorer cette mise. Veuillez saisir une valeur moins élevée.", ColorConstants.Red);
      DrawStartingPage();
      return;
    }

    bet = (uint)playerInput;
    playerOne.oid.PlaySound("it_coins");
    DrawStartingPage();
  }

  private bool HandleGold(PlayerSystem.Player player, bool takeGold = false)
  {
    if (player.oid.LoginCreature.Gold >= bet)
    {
      if (takeGold)
        player.oid.LoginCreature.TakeGold((int)bet);
      return true;
    }
    else if (player.oid.LoginCreature.Gold + player.bankGold >= bet)
    {
      if (takeGold)
      {
        player.bankGold -= (int)(bet - player.oid.LoginCreature.Gold);
        player.oid.LoginCreature.TakeGold((int)player.oid.LoginCreature.Gold);
      }
      return true;
    }

    return false;
  }

  private void SetDefaultMinimumBet()
  {
    int minimumGoldAvailable = 0;

    int p1Gold = (int)playerOne.oid.LoginCreature.Gold;
    if (playerOne.bankGold > 0)
      p1Gold += playerOne.bankGold;

    int p2Gold = (int)playerTwo.oid.LoginCreature.Gold;
    if (playerTwo.bankGold > 0)
      p2Gold += playerTwo.bankGold;

    if (p1Gold < p2Gold)
    {
      minimumGoldAvailable = p1Gold;

      if (minimumGoldAvailable < 2)
      {
        playerOne.menu.Close();
      }

      playerOne.oid.SendServerMessage("Vous n'avez pas assez d'or pour honorer la mise, celle-ci a donc été réduite à la mise minimale possible.", ColorConstants.Orange);
      playerTwo.oid.SendServerMessage($"{playerOne.oid.LoginCreature.Name} n'ayant pas assez d'or pour honorer la mise, celle-ci a donc été réduite à la mise minimale possible.", ColorConstants.Orange);
    }
    else
    {
      minimumGoldAvailable = p2Gold;

      if (minimumGoldAvailable < 2)
      {
        playerTwo.menu.Close();
      }

      playerTwo.oid.SendServerMessage("Vous n'avez pas assez d'or pour honorer la mise, celle-ci a donc été réduite à la mise minimale possible.", ColorConstants.Orange);
      playerOne.oid.SendServerMessage($"{playerTwo.oid.LoginCreature.Name} n'ayant pas assez d'or pour honorer la mise, celle-ci a donc été réduite à la mise minimale possible.", ColorConstants.Orange);
    }

    bet = (uint)minimumGoldAvailable - 1;
  }

  private async void WaitOpponent()
  {
    playerOne.menu.Clear();
    playerOne.menu.titleLines.Add("En attente d'un adversaire ...");

    playerOne.menu.choices.Add((
        "Quitter",
        () => playerOne.menu.Close()
      ));

    playerOne.menu.Draw();

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Task gameEnded = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").HasValue, tokenSource.Token);
    Task playerJoined = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_AVAILABLE_SLOTS").Value == 0, tokenSource.Token);

    await NwTask.WhenAny(gameEnded, playerJoined);
    tokenSource.Cancel();

    if (gameEnded.IsCompletedSuccessfully)
      return;

    if (!PlayerSystem.Players.TryGetValue(diceBoard.GetObjectVariable<LocalVariableObject<NwCreature>>("_PLAYER_TWO").Value, out PlayerSystem.Player player2))
      return;

    playerTwo = player2;
    StartGame();
  }
  private void StartGame()
  {
    DrawStartMenu(playerOne, playerTwo);
    DrawStartMenu(playerTwo, playerOne);
  }
  private void DrawStartMenu(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    player.menu.Clear();

    player.menu.titleLines = new List<string>() {
          $"La mise d'entrée de cette partie est de {bet} pièce(s) d'or.",
          $"Votre adversaire est {opponent.oid.LoginCreature.Name}.",
          "Acceptez-vous le défi ?",
          };

    player.menu.choices.Add((
      "C'est parti !",
      () => DrawInitialDiceRollMenu(player, opponent)
    ));

    player.menu.choices.Add((
      "Hors de question !",
      () => player.menu.Close()
    ));

    gameState = 1;
  }
  private async void DrawInitialDiceRollMenu(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    if (!HandleGold(player))
      SetDefaultMinimumBet();

    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_GAME_STARTED").Value = 1;

    player.menu.Clear();
    player.menu.titleLines = new List<string>() {
        $"En attente du choix adverse",
      };
    player.menu.Draw();

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Task gameCancelled = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").HasValue, tokenSource.Token);
    Task gameStarted = NwTask.WaitUntil(() => opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_GAME_STARTED").HasValue, tokenSource.Token);

    await NwTask.WhenAny(gameCancelled, gameStarted);
    tokenSource.Cancel();

    if (gameCancelled.IsCompletedSuccessfully)
      return;

    if (!HandleGold(player, true))
      SetDefaultMinimumBet();

    player.menu.Clear();

    player.menu.titleLines = new List<string>() {
          "Lancez les dés lorsque vous vous sentez prêt !",
          $"La mise est de {bet} pièce(s) d'or."
          };

    for (int i = 1; i < 6; i++)
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_DICE_{i}").Value = NwRandom.Roll(MiscUtils.random, 6);

    player.menu.choices.Add((
      $"{player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
      () => WaitPlayerRolls(player, opponent)
    ));

    player.menu.Draw();
    GetPlayerInitialDiceRoll(player, opponent);
  }
  private async void GetPlayerInitialDiceRoll(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Task gameEnded = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").HasValue, tokenSource.Token);
    Task diceRolled = NwTask.WaitUntil(() => player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_ROLLED").HasValue, tokenSource.Token);
    Task waitingForSelection = NwTask.Delay(TimeSpan.FromSeconds(0.2), tokenSource.Token);

    await NwTask.WhenAny(gameEnded, diceRolled, waitingForSelection);
    tokenSource.Cancel();

    if (diceRolled.IsCompletedSuccessfully || gameEnded.IsCompletedSuccessfully)
      return;

    player.menu.choices.Clear();

    for (int i = 1; i < 6; i++)
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_DICE_{i}").Value = NwRandom.Roll(MiscUtils.random, 6);

    player.menu.choices.Add((
      $"{player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
      () => WaitPlayerRolls(player, opponent)
    ));

    player.menu.DrawText();

    GetPlayerInitialDiceRoll(player, opponent);
  }
  private async void WaitPlayerRolls(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_ROLLED").Value = 1;

    player.menu.Clear();

    player.menu.titleLines = new List<string>() {
        $"Vous : {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        "En attente du lancé adverse."
      };

    player.menu.Draw();

    playerOne.oid.PlaySound("it_dice");

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Task gameEnded = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").HasValue, tokenSource.Token);
    Task diceRolled = NwTask.WaitUntil(() => opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_ROLLED").HasValue, tokenSource.Token);

    await NwTask.WhenAny(gameEnded, diceRolled);
    tokenSource.Cancel();

    if (gameEnded.IsCompletedSuccessfully)
      return;

    DrawRaiseBetMenu(player, opponent);
  }
  private void DrawRaiseBetMenu(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    player.menu.Clear();

    player.menu.titleLines = new List<string>() {
        $"Vous : {IdentifyBestHand(player).ToDescription()} - {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        $"{opponent.oid.LoginCreature.Name} : {IdentifyBestHand(opponent).ToDescription()} - {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        $"La mise est de {bet}. Souhaitez-vous le faire monter ?"
      };

    player.menu.choices.Add((
        $"Je relance de ...",
        () => GetRaise(player, opponent)
      ));

    player.menu.choices.Add((
        $"Je passe.",
        () => WaitForOpponentRaise(player, opponent)
      ));

    player.menu.Draw();
  }
  private async void GetRaise(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    player.menu.Clear();

    player.menu.titleLines = new List<string>() {
        $"La mise actuelle est de {bet}",
        "De combien souhaitez-vous relancer ?",
        "(La valeur saisie sera ajoutée à la mise actuelle)"
      };

    player.menu.Draw();

    bool awaitedValue = await player.WaitForPlayerInputInt();

    if (awaitedValue)
    {
      player.oid.PlaySound("it_coins");
      WaitForOpponentRaise(player, opponent);
    }
  }
  private async void WaitForOpponentRaise(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    int playerInput = 1;

    if (player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").HasValue)
    {
      playerInput = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value);
      if (playerInput < 1)
        playerInput = 1;
    }

    if (!HandleGold(player))
    {
      playerOne.oid.SendServerMessage("Vous n'avez pas assez d'or pour honorer la nouvelle mise. Votre proposition a donc été réduite à 1", ColorConstants.Orange);
      playerInput = 1;
    }

    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_ROLLED").Delete();
    player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_BET_RAISED").Value = playerInput;

    player.menu.Clear();
    player.menu.titleLines = new List<string>() {
        $"En attente du choix adverse",
      };
    player.menu.Draw();

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Task gameEnded = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").HasValue, tokenSource.Token);
    Task opponentRaised = NwTask.WaitUntil(() => opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_BET_RAISED").HasValue, tokenSource.Token);

    await NwTask.WhenAny(gameEnded, opponentRaised);
    tokenSource.Cancel();

    if (gameEnded.IsCompletedSuccessfully)
      return;

    int opponentInput = opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_BET_RAISED").Value;
    player.oid.SendServerMessage($"{opponent.oid.LoginCreature.Name} souhaite monter la mise de {opponentInput.ToString()}.", ColorConstants.Orange);

    if (playerInput >= opponentInput)
      bet += (uint)opponentInput / 2;
    else
      bet += (uint)playerInput / 2;


    Task waitForSynchro = NwTask.Run(async () =>
    {
      await NwTask.NextFrame();
      player.oid.SendServerMessage($"La relance moins-disante est sélectionnée. La mise est désormais de {bet.ToString()}.", ColorConstants.Orange);
      HandleGold(player, true);
      DrawRerollMenu(player, opponent);
    });
  }
  private void DrawRerollMenu(PlayerSystem.Player player, PlayerSystem.Player opponent, int selectedDice = 0)
  {
    if (selectedDice > 0)
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_REROLL_DICE_{selectedDice}").Value = 1;

    player.menu.Clear();

    player.menu.titleLines = new List<string>() {
        $"Vous : {IdentifyBestHand(player).ToDescription()} - {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        $"{opponent.oid.LoginCreature.Name} : {IdentifyBestHand(opponent).ToDescription()} - {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        $"La mise est de {bet}. Souhaitez-vous relancer certains dés ?"
      };

    for (int i = 1; i < 6; i++)
    {
      int currentValue = i;
      if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_REROLL_DICE_{currentValue}").HasNothing)
      {
        player.menu.choices.Add((
          $"{currentValue} - {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_DICE_{currentValue}").Value}",
          () => DrawRerollMenu(player, opponent, currentValue)
        ));
      }
    }

    player.menu.choices.Add((
        $"Valider la sélection et relancer",
        () => DoReroll(player, opponent)
      ));

    player.menu.Draw();
  }
  private void DoReroll(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    for (int i = 1; i < 6; i++)
    {
      if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_REROLL_DICE_{i}").HasValue)
      {
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_DICE_{i}").Value = player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_REROLL_DICE_{i}").Value;
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_DICE_POKER_REROLL_DICE_{i}").Delete();
      }
    }

    WaitOpponentReroll(player, opponent);
  }
  private async void WaitOpponentReroll(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    player.menu.Clear();

    player.menu.titleLines = new List<string>() {
        $"Vous : {IdentifyBestHand(player).ToDescription()} - {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        $"{opponent.oid.LoginCreature.Name} : {IdentifyBestHand(opponent).ToDescription()} - {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        $"La mise est de {bet}.",
        $"En attente de la relance de l'adversaire..."
      };

    player.menu.Draw();

    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_REROLLED").Value = 1;
    playerOne.oid.PlaySound("it_dice");

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Task gameEnded = NwTask.WaitUntil(() => diceBoard.GetObjectVariable<LocalVariableInt>("_POKER_DICE_GAME_ENDED").HasValue, tokenSource.Token);
    Task opponentRerolled = NwTask.WaitUntil(() => opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_REROLLED").HasValue, tokenSource.Token);

    await NwTask.WhenAny(gameEnded, opponentRerolled);
    tokenSource.Cancel();

    if (gameEnded.IsCompletedSuccessfully)
      return;


    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_BET_RAISED").Delete();

    player.menu.Clear();

    int gameResult = WinOrLoss(player, opponent);
    string resultDisplay = "";

    switch (gameResult)
    {
      case 0:
        resultDisplay = $"Défaite ! {opponent.oid.LoginCreature.Name} empoche la mise de {bet}";
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessateNegative));
        opponent.oid.LoginCreature.GiveGold((int)bet);
        break;
      case 1:
        resultDisplay = $"Victoire ! Vous empochez la mise de {bet}";
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
        player.oid.LoginCreature.GiveGold((int)bet);
        break;
      default:
        resultDisplay = $"Match nul ! La mise de {bet} est restitué à chacun des joueurs";
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessateNeutral));
        player.oid.LoginCreature.GiveGold((int)bet / 4);
        opponent.oid.LoginCreature.GiveGold((int)bet / 4);
        break;
    }

    player.menu.titleLines = new List<string>() {
        $"Vous : {IdentifyBestHand(player).ToDescription()} - {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        $"{opponent.oid.LoginCreature.Name} : {IdentifyBestHand(opponent).ToDescription()} - {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value} | {opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value}",
        resultDisplay
      };

    player.menu.Draw();

    gameState = 3;
  }
  private HandType IdentifyBestHand(PlayerSystem.Player player)
  {
    int[] playerHand = new int[] { player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value, player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value, player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value, player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value, player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value };

    if (IsFiveOfAKind(playerHand))
      return HandType.FiveOfAKind;
    if (IsFourOfAKind(playerHand))
      return HandType.FourOfAKind;
    if (IsFullHouse(playerHand))
      return HandType.FullHouse;
    if (IsHighStraight(playerHand))
      return HandType.HighStraight;
    if (IsLowStraight(playerHand))
      return HandType.LowStraight;
    if (IsThreeOfAKind(playerHand))
      return HandType.ThreeOfAKind;
    if (IsTwoPair(playerHand))
      return HandType.TwoPairs;
    if (IsPair(playerHand))
      return HandType.Pair;

    return HandType.Nothing;
  }

  private bool IsFiveOfAKind(int[] playerHand)
  {
    return playerHand.GroupBy(h => h)
              .Where(g => g.Count() == 5)
              .Any();
  }
  private bool IsFourOfAKind(int[] playerHand)
  {
    return playerHand.GroupBy(h => h)
              .Where(g => g.Count() == 4)
              .Any();
  }
  private bool IsThreeOfAKind(int[] playerHand)
  {
    return playerHand.GroupBy(h => h)
              .Where(g => g.Count() == 3)
              .Any();
  }
  private bool IsTwoPair(int[] playerHand)
  {
    return playerHand.GroupBy(h => h)
              .Where(g => g.Count() == 2)
              .Count() == 2;
  }
  private bool IsPair(int[] playerHand)
  {
    return playerHand.GroupBy(h => h)
              .Where(g => g.Count() == 2)
              .Count() == 1;
  }
  private bool IsFullHouse(int[] playerHand)
  {
    return IsPair(playerHand) && IsThreeOfAKind(playerHand);
  }
  private bool IsHighStraight(int[] playerHand)
  {
    return playerHand.Contains(6) && playerHand.Contains(5) && playerHand.Contains(4) && playerHand.Contains(3) && playerHand.Contains(2);
  }
  private bool IsLowStraight(int[] playerHand)
  {
    return playerHand.Contains(5) && playerHand.Contains(4) && playerHand.Contains(3) && playerHand.Contains(2) && playerHand.Contains(1);
  }
  private enum HandType
  {
    [Description("Rien")]
    Nothing = 0,
    [Description("Paire")]
    Pair = 1,
    [Description("Deux_Paires")]
    TwoPairs = 2,
    [Description("Trois_à_la_suite")]
    ThreeOfAKind = 3,
    [Description("Petite_Suite")]
    LowStraight = 4,
    [Description("Grande_Suite")]
    HighStraight = 5,
    [Description("Brelan")]
    FullHouse = 6,
    [Description("Quatre_à_la_suite")]
    FourOfAKind = 7,
    [Description("Cinq_à_la_suite")]
    FiveOfAKind = 8,
  }
  private int WinOrLoss(PlayerSystem.Player player, PlayerSystem.Player opponent)
  {
    HandType playerHandType = IdentifyBestHand(player);
    HandType opponentHandType = IdentifyBestHand(opponent);

    if ((int)playerHandType > (int)opponentHandType)
      return 1;
    else if ((int)playerHandType < (int)opponentHandType)
      return 0;

    int playerSum = player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value + player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value + player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value + player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value + player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value;
    int opponentSum = opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_1").Value + opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_2").Value + opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_3").Value + opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_4").Value + opponent.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_DICE_POKER_DICE_5").Value;

    if (playerSum > opponentSum)
      return 1;
    if (playerSum < opponentSum)
      return 0;

    return 2;
  }
}
}
