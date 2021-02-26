using System.Collections.Generic;
using System.Threading.Tasks;
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
    private void HandleMoneyDepositSelection(Player player)
    {
      player.menu.Clear();
      uint availableGold = player.oid.Gold;

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

        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;

        Task playerInput = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue);
          if (player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").HasNothing)
            HandleValidateDeposit(player);
          else
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();
        });
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleDepositAll(Player player)
    {
      player.menu.Clear();

      player.bankGold += (int)player.oid.Gold;
      player.oid.TakeGold((int)player.oid.Gold);

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
      uint availableGold = player.oid.Gold;

      if (player.setValue <= 0)
      {
        player.menu.titleLines.Add($"Plait-il ? Je n'ai pas bien compris.");
        player.menu.choices.Add(($"Valider.", () => HandleValidateDeposit(player)));
      }
      else if (player.setValue > availableGold)
      {
        player.menu.titleLines = new List<string> {
          $"Rien qu'à l'odeur, je sais que tu as {availableGold} pièces d'or sur toi.",
          $"Pas {player.setValue}. Fini de faire le mariole ?"
        };
        player.menu.choices.Add(($"Valider.", () => HandleValidateDeposit(player)));
      }
      else if (player.setValue < availableGold)
      {
        player.menu.titleLines = new List<string> {
          $"{player.setValue} ? Tu pourrais faire mieux.",
          "Enfin, c'est toujours ça..."
        };

        player.oid.TakeGold(player.setValue);
        player.bankGold += player.setValue;
      }
      else if (player.setValue == availableGold)
      {
        player.menu.titleLines = new List<string> {
          $"Oh, oui, toooout. Donnez moi tooooout !",
          "Vous pouvez me faire confiance ... oui, confiance ..."
        };

        player.oid.TakeGold((int)player.oid.Gold);
        player.bankGold += player.setValue;
      }

      player.setValue = 0;
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleMoneyWithdrawalSelection(Player player)
    {
      player.menu.Clear();

      if (player.bankGold == 1)
      {
        player.menu.titleLines = new List<string> {
          $"Quoi ? Tu me casses les roubignoles pour une pièce ?",
          "Du balai, mendiant."
        };
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }
      else
      {
        player.menu.titleLines = new List<string>
        {
          $"Votre solde actuel est de {player.bankGold} pièces d'or.",
          "Vous êtes sur de vouloir courir le risque de nous les retirer ?",
          "(Dites moi simplement la valeur souhaitée à l'oral)"
        };

        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;

        Task playerInput = NwTask.Run(async () =>
        {
          await NwTask.WaitUntilValueChanged(() => player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue);
          if (player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").HasNothing)
            HandleValidateWithdrawal(player);
          else
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();
        });
        
        player.menu.choices.Add(($"Tout retirer.", () => HandleWithdrawAll(player)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleValidateWithdrawal(Player player)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.titleLines = new List<string> {
          $"Plait-il ? Je n'ai pas bien compris.",
          "(Utilisez la commande !set X avant de valider votre choix)"
        };
        player.menu.choices.Add(($"Valider.", () => HandleValidateWithdrawal(player)));
      }
      else if (player.setValue > player.bankGold)
      {
        player.menu.titleLines = new List<string> {
          "Ah, encore un petit rigolo qui se croit malin.",
          $"Gamin, t'as {player.bankGold} pièces d'or dans mon coffre.",
          $"Pas {player.setValue}."
        };
        player.menu.choices.Add(($"Valider.", () => HandleValidateWithdrawal(player)));
      }
      else if (player.setValue < player.bankGold)
      {
        player.menu.titleLines = new List<string> {
          $"{player.setValue} ? Pas très malin par les temps qui courent ...",
          "un coup de gourdin est si vite arrivé. Enfin, si vous y tenez ..."
        };

        player.oid.GiveGold(player.setValue);
        player.bankGold -= player.setValue;
      }
      else if (player.setValue == player.bankGold)
      {
        player.menu.titleLines = new List<string> {
          "QUOI ?! Mais tu es fada ma parole ? Tu veux ma ruine ?",
          "Non, non, soit raisonnable. Tout retirer serait pure folie !"
        };
        
        Task playerInput = NwTask.Run(async () =>
        {
          await NwTask.WaitUntilValueChanged(() => player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue);
          if (player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").HasNothing)
            HandleValidateWithdrawal(player);
          else
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();
        });
      }

      player.setValue = 0;
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

        player.oid.GiveGold(player.bankGold - 1);
        player.bankGold = 1;
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
