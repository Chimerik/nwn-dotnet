using System.Collections.Generic;
using NWN.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Bank
  {
    public Bank(Player player)
    {
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Bonjour et bienvenue chez TRUANT et TRUAND associés,",
        "que nous vaut le plaisir de votre visite ?",
        "",
        $"Votre solde actuel est de {player.bankGold}"
      };

      player.menu.choices.Add(($"Je voudrais déposer de l'or", () => HandleMoneyDepositSelection(player)));
      if (player.bankGold > 0)
        player.menu.choices.Add(($"Je voudrais retirer de l'or", () => HandleMoneyWithdrawalSelection(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void HandleMoneyDepositSelection(Player player)
    {
      player.menu.Clear();
      uint availableGold = player.oid.LoginCreature.Gold;

      if (availableGold < 1)
      {
        player.menu.titleLines = new List<string> {
          $"Je ne sens pas l'odeur de la moindre pièce d'or sur toi.",
          "Du balai, va-nu-pied !"
        };
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }
      else if (availableGold == 1)
      {
        player.menu.titleLines = new List<string> {
          "Une pièce d'or ? Voilà qui ne m'émeut guère.",
          "Tu peux la garder, on ne va pas aller bien loin avec ça (sans-le-sous)."
        };
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }
      else
      {
        player.menu.titleLines = new List<string> {
          $"Mes narines frémissent à l'odeur des {availableGold} pièces d'or qui doivent te peser bien trop lourd.",
          "De combien puis-je te débarrasser ? (Dites moi simplement la valeur correspondante à l'oral)"
        };
        player.menu.choices.Add(($"Tout déposer.", () => HandleDepositAll(player)));
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));

        bool awaitedValue = await player.WaitForPlayerInputInt();

        if (awaitedValue)
          HandleValidateDeposit(player);
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleDepositAll(Player player)
    {
      player.menu.Clear();

      player.bankGold += (int)player.oid.LoginCreature.Gold;
      player.oid.LoginCreature.TakeGold((int)player.oid.LoginCreature.Gold);

      player.menu.titleLines = new List<string> {
        $"Oh, oui, toooout. Donnez moi tooooout !",
        "Vous pouvez me faire confiance ... oui, confiance ..."
      };

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleValidateDeposit(Player player)
    {
      player.menu.Clear();
      uint availableGold = player.oid.LoginCreature.Gold;
      int playerInput = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));
      player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();

      if (playerInput <= 0)
      {
        player.menu.titleLines.Add($"Plait-il ? Je n'ai pas bien compris.");
        player.menu.choices.Add(($"Valider.", () => HandleMoneyDepositSelection(player)));

      }
      else if (playerInput > availableGold)
      {
        player.menu.titleLines = new List<string> {
          $"Rien qu'à l'odeur, je sais que tu as {availableGold} pièces d'or sur toi.",
          $"Pas {playerInput}. Fini de faire le mariole ?"
        };
        player.menu.choices.Add(($"Valider.", () => HandleMoneyDepositSelection(player)));
      }
      else if (playerInput < availableGold)
      {
        player.menu.titleLines = new List<string> {
          $"{playerInput} ? Tu pourrais faire mieux.",
          "Enfin, c'est toujours ça..."
        };

        player.oid.LoginCreature.TakeGold(playerInput);
        player.bankGold += playerInput;
      }
      else if (playerInput == availableGold)
      {
        player.menu.titleLines = new List<string> {
          $"Oh, oui, toooout. Donnez moi tooooout !",
          "Vous pouvez me faire confiance ... oui, confiance ..."
        };

        player.oid.LoginCreature.TakeGold((int)player.oid.LoginCreature.Gold);
        player.bankGold += playerInput;
      }

      playerInput = Config.invalidInput;
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void HandleMoneyWithdrawalSelection(Player player)
    {
      player.menu.Clear();

      if (player.bankGold == 1)
      {
        player.menu.titleLines = new List<string> {
          $"Quoi ? Tu me casses les roubignoles pour une pièce ?",
          "Du balai, mendiant."
        };

        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
        player.menu.choices.Add(("Quitter", () => player.menu.Close()));
        player.menu.Draw();
      }
      else
      {
        player.menu.titleLines = new List<string>
        {
          $"Votre solde actuel est de {player.bankGold} pièces d'or.",
          "Vous êtes sur de vouloir courir le risque de nous les retirer ?",
          "(Dites moi simplement la valeur souhaitée à l'oral)"
        };

        player.menu.choices.Add(($"Tout retirer.", () => HandleWithdrawAll(player)));
        player.menu.choices.Add(("Quitter", () => player.menu.Close()));
        player.menu.Draw();

        bool awaitedValue = await player.WaitForPlayerInputInt();

        if (awaitedValue)
          HandleValidateWithdrawal(player);
      }
    }
    private void HandleValidateWithdrawal(Player player)
    {
      player.menu.Clear();
      int playerInput = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));
      player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();

      if (playerInput <= 0)
      {
        player.oid.SendServerMessage("La valeur saisie doit être supérieure à 0.", Color.RED);
        HandleMoneyWithdrawalSelection(player);
        return;
      }
      else if (playerInput > player.bankGold)
      {
        player.menu.titleLines = new List<string> {
          "Ah, encore un petit rigolo qui se croit malin.",
          $"Gamin, t'as {player.bankGold} pièces d'or dans mon coffre.",
          $"Pas {playerInput}."
        };
        player.menu.choices.Add(($"Valider.", () => HandleMoneyWithdrawalSelection(player)));
      }
      else if (playerInput < player.bankGold)
      {
        player.menu.titleLines = new List<string> {
          $"{playerInput} ? Pas très malin par les temps qui courent ...",
          "un coup de gourdin est si vite arrivé. Enfin, si vous y tenez ..."
        };

        player.oid.LoginCreature.GiveGold(playerInput);
        player.bankGold -= playerInput;
      }
      else if (playerInput == player.bankGold)
      {
        player.menu.titleLines = new List<string> {
          "QUOI ?! Mais tu es fada ma parole ? Tu veux ma ruine ?",
          "Bon, bon, mais on garde une pièce d'or pour que le compte reste ouvert !"
        };

        player.oid.LoginCreature.GiveGold(player.bankGold - 1);
        player.bankGold = 1;
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleWithdrawAll(Player player)
    {
      player.menu.Clear();

      if (player.bankGold <= 1)
        player.menu.titleLines = new List<string> {
          $"Génial, encore un petit rigolo, c'est pas veine ...",
          "Vous me devez plus que ce que vous pouvez bien retirer, pauvre amie ..."
        };
      else
      {
        player.menu.titleLines = new List<string> {
          "QUOI ?! Mais tu es fada ma parole ? Tu veux ma ruine ?",
          "Non, non, soit raisonnable. Tout retirer serait pure folie !",
          "Bon bon, si tu insistes ... je vais quand même garder une pièce en sureté, au cas. Un coup de gourdin est si vite arrivé ..."
        };

        player.oid.LoginCreature.GiveGold(player.bankGold - 1);
        player.bankGold = 1;
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
