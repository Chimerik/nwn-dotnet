using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class BankGold
  {
    public BankGold(Player player)
    {
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.menu.Clear();
      player.menu.title = $"Un gros tas de pièces d'or se trouve devant vous. Etrangement, personne alentour pour le surveiller. Que faites-vous ?";
      player.menu.choices.Add(($"Prendre 500 pièces. Après tout, il serait idiot de ne pas se remplir les poches.", () => HandleStealBankGold(player)));
      player.menu.choices.Add(("S'éloigner. Un tas d'or dans une banque sans personne autour, c'est suspect.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleStealBankGold(Player player)
    {
      player.menu.Clear();
      NWScript.GiveGoldToCreature(player.oid, 500);
      player.bankGold -= 500 + 500 * 30 / 100;

      player.menu.title = $"En voilà un gain simple et facile !";

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleValidateDeposit(PlayerSystem.Player player)
    {
      player.menu.Clear();
      int availableGold = NWScript.GetGold(player.oid);

      if (player.setValue <= 0)
      {
        player.menu.title = $"Plait-il ? Je n'ai pas bien compris. (Utilisez la commande !set X avant de valider votre choix)";
        player.menu.choices.Add(($"Valider.", () => HandleValidateDeposit(player)));
      }
      else if (player.setValue > availableGold)
      {
        player.menu.title = $"Rien qu'à l'odeur, je sais que tu as {availableGold} pièces d'or sur toi. Pas {player.setValue}. Fini de faire le mariole ?";
        player.menu.choices.Add(($"Valider.", () => HandleValidateDeposit(player)));
      }
      else if (player.setValue < availableGold)
      {
        player.menu.title = $"{player.setValue} ? Tu pourrais faire mieux. Enfin, c'est toujours ça...";
        CreaturePlugin.SetGold(player.oid, availableGold - player.setValue);
        player.bankGold += player.setValue;
      }
      else if (player.setValue == availableGold)
      {
        player.menu.title = $"Oh, oui, toooout. Donnez moi tooooout ! Vous pouvez me faire confiance ... oui, confiance ...";
        CreaturePlugin.SetGold(player.oid, 0);
        player.bankGold += player.setValue;
      }

      player.setValue = 0;
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleMoneyWithdrawalSelection(PlayerSystem.Player player)
    {
      player.menu.Clear();

      if (player.bankGold == 1)
      {
        player.menu.title = $"Quoi ? Tu me casses les roubignoles pour une pièce ? Du balai, mendiant.";
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }
      else
      {
        player.menu.title = $"Votre solde actuel est de {player.bankGold} pièces d'or. Vous êtes sur de vouloir courir le risque de nous les retirer ? (Utilisez la commande !set X puis validez votre choix)";
        player.menu.choices.Add(($"Valider.", () => HandleValidateWithdrawal(player)));
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleValidateWithdrawal(PlayerSystem.Player player)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.title = $"Plait-il ? Je n'ai pas bien compris. (Utilisez la commande !set X avant de valider votre choix)";
        player.menu.choices.Add(($"Valider.", () => HandleValidateWithdrawal(player)));
      }
      else if (player.setValue > player.bankGold)
      {
        player.menu.title = $"Ah, encore un petit rigolo qui se croit malin. Gamin, t'as {player.bankGold} pièces d'or dans mon coffre. Pas {player.setValue}.";
        player.menu.choices.Add(($"Valider.", () => HandleValidateWithdrawal(player)));
      }
      else if (player.setValue < player.bankGold)
      {
        player.menu.title = $"{player.setValue} ? Pas très malin par les temps qui courent ... un coup de gourdin est si vite arrivé. Enfin, si vous y tenez ...";
        NWScript.GiveGoldToCreature(player.oid, player.setValue);
        player.bankGold -= player.setValue;
      }
      else if (player.setValue == player.bankGold)
      {
        player.menu.title = $"QUOI ?! Mais tu es fada ma parole ? Tu veux ma ruine ? Non, non, soit raisonnable. Tout retirer serait pure folie !";
        player.menu.choices.Add(($"Valider.", () => HandleValidateWithdrawal(player)));
      }

      player.setValue = 0;
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
