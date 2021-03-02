using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteStocksMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
        DrawMainPage(player);
    }

    private static void DrawMainPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir un type de ressource :");
      player.menu.choices.Add((
        "Métal",
        () => DrawMetalPage(player)
      ));
      player.menu.choices.Add((
        "Bois",
        () => DrawBoisPage(player)
      ));
      player.menu.choices.Add((
        "Peaux",
        () => DrawPeltPage(player)
      ));

      player.menu.Draw();
    }

    private static void DrawMetalPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir un type d'établi :");
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

    private static void DrawFoundryPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de métaux non raffinés :");

      foreach(var entry in oresDictionnary)
        if(player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Value.name}: {playerStock}");

      player.menu.choices.Add((
        "Retour",
        () => DrawMetalPage(player)
      ));

      player.menu.Draw();
    }

    private static void DrawForgePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de métaux raffinés :");

      foreach (var entry in mineralDictionnary)
        if(player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Value.name}: {playerStock}");

      player.menu.choices.Add((
        "Retour",
        () => DrawMetalPage(player)
      ));

      player.menu.Draw();
    }
    private static void DrawBoisPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir un type d'établi :");
      player.menu.choices.Add((
        "Scierie",
        () => DrawScieriePage(player)
      ));
      player.menu.choices.Add((
        "Ebénisterie",
        () => DrawEbenisteriePage(player)
      ));
      player.menu.choices.Add((
        "Retour",
        () => DrawMainPage(player)
      ));

      player.menu.Draw();
    }
    private static void DrawScieriePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de bois brut :");

      foreach (var entry in woodDictionnary)
        if(player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Value.name}: {playerStock}");

      player.menu.choices.Add((
        "Retour",
        () => DrawBoisPage(player)
      ));

      player.menu.Draw();
    }
    private static void DrawEbenisteriePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de planches de bois raffinées :");

      foreach (var entry in plankDictionnary)
        if(player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Key.ToDescription()}: {playerStock}");

      player.menu.choices.Add((
        "Retour",
        () => DrawBoisPage(player)
      ));

      player.menu.Draw();
    }
    private static void DrawPeltPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir un type d'établi :");
      player.menu.choices.Add((
        "Tannerie",
        () => DrawTanneriePage(player)
      ));
      player.menu.choices.Add((
        "Maroquinerie",
        () => DrawMaroquineriePage(player)
      ));
      player.menu.choices.Add((
        "Retour",
        () => DrawMainPage(player)
      ));

      player.menu.Draw();
    }
    private static void DrawTanneriePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de peaux brutes :");

      foreach (var entry in peltDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Key.ToDescription()}: {playerStock}");

      player.menu.choices.Add((
        "Retour",
        () => DrawPeltPage(player)
      ));

      player.menu.Draw();
    }
    private static void DrawMaroquineriePage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici votre stock de cuirs raffinés :");

      foreach (var entry in leatherDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Key.ToDescription()}: {playerStock}");

      player.menu.choices.Add((
        "Retour",
        () => DrawPeltPage(player)
      ));

      player.menu.Draw();
    }
  }
}
