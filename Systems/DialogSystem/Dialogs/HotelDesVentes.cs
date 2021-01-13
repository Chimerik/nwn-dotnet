using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class HotelDesVentes
  {
    public HotelDesVentes(Player player)
    {
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Bientôt, il me sera possible de gérer des ordres d'achat et de vente pour vous.",
        "Mais le moment n'est pas encore venu ..."
      };

      player.menu.choices.Add(("Quitter [En cours de développement]", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
