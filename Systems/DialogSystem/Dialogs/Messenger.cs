using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Messenger
  {
    public Messenger(Player player)
    {
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Salutations. Je suis à votre service pour l'envoi et la réception de message écrits.",
        "Je puis également vous assister dans la création d'ouvrages.",
        "Que souhaitez-vous faire ?"
      };

      /*player.menu.choices.Add(("Lire mes messages.", () => player.menu.Close()));
      player.menu.choices.Add(("Rédiger un message.", () => player.menu.Close()));
      player.menu.choices.Add(("Rédiger un ouvrage.", () => player.menu.Close()));*/
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
