

using System.Collections.Generic;
using System.Numerics;

namespace NWN.Systems.Alchemy
{
  class AlchemyDialog
  {
    PlayerSystem.Player player;
    Vector2 tablePosition;
    public AlchemyDialog(PlayerSystem.Player player)
    {
      this.player = player;
      tablePosition = AlchemySystem.center;
      this.DrawWelcomePage();
    }
    private void DrawWelcomePage()
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Le chaudron est rempli d'une eau neutre et limpide.",
        "Que souhaitez-vous faire ?"
      };

      player.menu.choices.Add(("Ajouter des ingrédients", () => AddIngredient()));
      player.menu.choices.Add(("Réduire des ingrédients en poudre", () => player.menu.Close()));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
