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
    private void DrawWelcomePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Bientôt, il me sera possible de transmettre et recevoir des messages pour vous.",
        "Mais le temps n'est pas encore venu ..."
      };

      player.menu.choices.Add(("Quitter [En cours de développement]", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
