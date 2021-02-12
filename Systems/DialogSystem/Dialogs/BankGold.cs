using System.Collections.Generic;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class BankGold
  {
    public BankGold(Player player)
    {
      DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Un gros tas de pièces d'or se trouve devant vous.",
        "Etrangement, personne alentour pour le surveiller.",
        "Que faites-vous ?"
      };
      player.menu.choices.Add(("Prendre 500 pièces. Après tout, il serait idiot de ne pas se remplir les poches.", () => HandleStealBankGold(player)));
      player.menu.choices.Add(("S'éloigner. Un tas d'or dans une banque sans personne autour, c'est suspect.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleStealBankGold(Player player)
    {
      player.menu.Clear();
      player.oid.GiveGold(500);
      player.bankGold -= 500 + 500 * 30 / 100;

      player.menu.titleLines.Add("En voilà un gain simple et facile !");

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
