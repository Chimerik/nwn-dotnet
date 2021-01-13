using System.Collections.Generic;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteHelpCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (Players.TryGetValue(ctx.oSender, out Player player))
      {
        DrawAllCommandsPage(player);
      }
    }

    private static void DrawAllCommandsPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici la liste de toutes les commandes disponibles :");

      foreach (KeyValuePair<string, Command> entry in commandDic)
      {
        player.menu.choices.Add((
           entry.Value.shortDesc,
           () => DrawSingleCommandPage(player, entry.Value.longDesc)
        ));
      }

      player.menu.Draw();
    }

    private static void DrawSingleCommandPage(Player player, string description)
    {
      player.menu.Clear();

      var descriptionLines = description.Split("\n");

      foreach (var line in descriptionLines)
      {
        player.menu.titleLines.Add(line);
      }

      player.menu.choices.Add((
        "Retour",
        () => DrawAllCommandsPage(player)
      ));

      player.menu.Draw();
    }
  }
}
