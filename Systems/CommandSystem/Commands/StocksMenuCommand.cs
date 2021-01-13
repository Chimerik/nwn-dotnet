using System;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteStocksMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      Player player;
      if (Players.TryGetValue(ctx.oSender, out player))
      {
        DrawMainPage(player);
      }
    }

    private static void DrawMainPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir un type de ressource :");
      player.menu.choices.Add((
        "Métal",
        () => DrawMetalPage(player)
      ));

      player.menu.Draw();
    }

    private static void DrawMetalPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir un type d'établis :");
      player.menu.choices.Add((
        "Fonderie",
        () => DrawFoundryPage(player)
      ));
      player.menu.choices.Add((
        "Forge",
        () => DrawForgePage(player)
      ));
      player.menu.choices.Add((
        "Retour",
        () => DrawMainPage(player)
      ));

      player.menu.Draw();
    }

    private static void DrawFoundryPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de métaux non raffinés :");

      foreach(var entry in oresDictionnary)
      {
        int playerStock = 0;
        player.materialStock.TryGetValue(entry.Value.name, out playerStock);

        player.menu.choices.Add((
          $"{entry.Value.name}: {playerStock}",
          () => { }
        ));
      }

      player.menu.choices.Add((
        "Retour",
        () => DrawMetalPage(player)
      ));

      player.menu.Draw();
    }

    private static void DrawForgePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de métaux raffinés :");

      foreach (var entry in mineralDictionnary)
      {
        int playerStock = 0;
        player.materialStock.TryGetValue(entry.Value.name, out playerStock);

        player.menu.choices.Add((
          $"{entry.Value.name}: {playerStock}",
          () => { }
        ));
      }

      player.menu.choices.Add((
        "Retour",
        () => DrawMetalPage(player)
      ));

      player.menu.Draw();
    }
  }
}
