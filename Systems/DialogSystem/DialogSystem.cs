using System;
using System.Collections.Generic;
using System.Text;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class DialogSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "diag_bank", HandleBankDialog },
        };

    private static int HandleBankDialog(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastSpeaker(), out player))
      {
        DrawWelcomePage(player);
      }
        
      return 0;
    }
    private static void DrawWelcomePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = $"Bonjour et bienvenue chez TRUANT et TRUAND associés, que nous vaut le plaisir de votre visite ? Votre solde actuel est de {player.bankGold}";
      player.menu.choices.Add(($"Je voudrais déposer de l'or", () => HandleMoneyDepositSelection(player)));
      if(player.bankGold > 0)
        player.menu.choices.Add(($"Je voudrais retirer de l'or", () => HandleMoneyWithdrawalSelection(player)));
      player.menu.choices.Add(("Quitter", () => HandleClose(player)));
      player.menu.Draw();
    }
    private static void HandleMoneyDepositSelection(PlayerSystem.Player player)
    {
      player.menu.Clear();
      int availableGold = NWScript.GetGold(player.oid);

      if (availableGold < 1)
      {
        player.menu.title = $"Je ne sens pas l'odeur de la moindre pièce d'or sur toi. Du balai, va-nu-pied !";
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }
      else if (availableGold == 1)
      {
        player.menu.title = $"Une pièce d'or ? Voilà qui ne m'émeut guère. Tu peux la garder, on ne va pas aller bien loin avec ça (sans-le-sous).";
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }
      else
      {
        player.menu.title = $"Mes narines frémissent à l'odeur des {availableGold} pièces d'or qui doivent te peser bien trop lourd. De combien puis-je te débarrasser ? (Utilisez la commande !set X puis validez votre choix)";
        player.menu.choices.Add(($"Valider.", () => HandleValidateDeposit(player)));
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }

      player.menu.choices.Add(("Quitter", () => HandleClose(player)));
      player.menu.Draw();
    }
    private static void HandleValidateDeposit(PlayerSystem.Player player)
    {
      player.menu.Clear();
      int availableGold = NWScript.GetGold(player.oid);

      if (player.setValue <= 0)
      {
        player.menu.title = $"Plait-il ? Je n'ai pas bien compris. (Utilisez la commande !set X avant de valider votre choix)";
        player.menu.choices.Add(($"Valider.", () => HandleValidateDeposit(player)));
      }
      else if(player.setValue > availableGold)
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
      player.menu.choices.Add(("Quitter", () => HandleClose(player)));
      player.menu.Draw();
    }
    private static void HandleMoneyWithdrawalSelection(PlayerSystem.Player player)
    {
      player.menu.Clear();

      if (player.bankGold == 1)
      {
        player.menu.title = $"Quoi ? Vous me casser les roubignoles pour une pièce ? Du balai, mendiant.";
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }
      else
      {
        player.menu.title = $"Votre solde actuel est de {player.bankGold} pièces d'or. Vous êtes sur de vouloir courir le risque de nous les retirer ? (Utilisez la commande !set X puis validez votre choix)";
        player.menu.choices.Add(($"Valider.", () => HandleValidateWithdrawal(player)));
        player.menu.choices.Add(($"Retour.", () => DrawWelcomePage(player)));
      }

      player.menu.choices.Add(("Quitter", () => HandleClose(player)));
      player.menu.Draw();
    }
    private static void HandleValidateWithdrawal(PlayerSystem.Player player)
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
      player.menu.choices.Add(("Quitter", () => HandleClose(player)));
      player.menu.Draw();
    }
    private static void HandleClose(PlayerSystem.Player player)
    {
      player.menu.Close();
    }
  }
}
