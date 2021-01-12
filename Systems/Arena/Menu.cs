using NWN.Core;
using static NWN.Systems.Arena.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class Menu
  {
    public static void Draw(Player player)
    {
      player.menu.Clear();
      player.menu.title = "Bienvenue dans l'arene de Similisse ! Que puis-je faire pour vous aujourd'hui ?";
      player.menu.choices.Add((
        "M'inscrire pour participer aux prochains combats",
        () => DrawSubcribePage(player)
      ));
      player.menu.choices.Add((
        "Depenser mes points de victoire pour acheter des récompenses",
        () => DrawRewardPage(player)
      ));
      player.menu.choices.Add((
        "Voir la liste des meilleurs combattants",
        () => DrawHighscoresPage(player)
      ));

      player.menu.Draw();
    }

    private static void DrawSubcribePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Veuillez choisir votre niveau de difficulté";
      player.menu.choices.Add((
         "1. C'est ma première fois, ne tapez pas trop fort s'il vous plaît !",
         () => DrawConfirmPage(player, Difficulty.Level1)
      ));

      player.menu.Draw();
    }

    private static void DrawConfirmPage(Player player, Difficulty difficulty)
    {
      player.menu.Clear();
      player.menu.title = $"Très bien, il vous en coûtera {Utils.GetGoldEntryCost(difficulty)} PO. Etes-vous prêt a rentrer dans l'arene ?";
      player.menu.choices.Add((
        "Absolument, je suis prêt à en découdre !",
        () => HandleConfirm(player, difficulty)
      ));
      player.menu.choices.Add((
        "Non, peut-être une autre fois.",
        () => player.menu.Close()
      ));

      player.menu.Draw();
    }

    private static void HandleConfirm(Player player, Difficulty difficulty)
    {
      player.PayOrBorrowGold(Utils.GetGoldEntryCost(difficulty));

      NWScript.CreateArea(PVE_ARENA_AREA_RESREF, "", $"Arène de {NWScript.GetName(player.oid)} - Niveau {difficulty}");
      // Tp le joueur dans la zone.
    }

    private static void DrawRewardPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Vous avez actuellement xxx points de victoire disponible. Voici la liste des récompenses disponibles :";
      player.menu.choices.Add((
        "Retour",
        () => Draw(player)
      ));
      player.menu.Draw();
    }

    private static void DrawHighscoresPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Voici la liste de nos meilleurs champions :";
      player.menu.choices.Add((
        "Retour",
        () => Draw(player)
      ));
      player.menu.Draw();
    }
  }
}
