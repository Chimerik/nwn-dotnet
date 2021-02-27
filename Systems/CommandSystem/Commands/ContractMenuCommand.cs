using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    /*private static void ExecuteContractMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        DrawMainContractPage(player);
      }
    }

    private static void DrawMainContractPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Que souhaitez-vous faire ?");
      player.menu.choices.Add((
        "Rédiger un nouveau contrat",
        () => WriteContractPage(player)
      ));
      player.menu.choices.Add((
        "Consulter mes contrats en attente",
        () => DrawCurrentContractPage(player)
      ));
      player.menu.choices.Add((
        "Quitter",
        () => player.menu.Close()
      )); 

      player.menu.Draw();
    }

    private static void WriteContractPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Quelle ressource souhaitez-vous faire figurer dans ce contrat ?");

      foreach (var entry in oresDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Value.name}: {playerStock}");

      foreach (var entry in mineralDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Value.name}: {playerStock}");

      foreach (var entry in woodDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Value.name}: {playerStock}");

      foreach (var entry in plankDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Key.ToDescription()}: {playerStock}");

      foreach (var entry in peltDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Key.ToDescription()}: {playerStock}");

      foreach (var entry in leatherDictionnary)
        if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
          player.menu.titleLines.Add($"* {entry.Key.ToDescription()}: {playerStock}");

      player.menu.choices.Add((
        "Retour",
        () => DrawMainContractPage(player)
      ));

      player.menu.Draw();
    }
    private static void HandleValidateMaterialSelection(PlayerSystem.Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
          $"Quelle quantité de {material} souhaitez-vous faire figurer dans ce contrat ?",
          "(Indiquez simplement la valeur à l'oral)"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue);
        if (player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value != Config.invalidInput)
          HandleSetupPriceContract(player, material);
        else
          player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();
      });
    }
    private static void HandleSetupPriceContract(PlayerSystem.Player player, string material)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.titleLines.Add($"La  valeur indiquée n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => WriteContractPage(player)));
      }
      else
      {
        if (player.setValue >= player.materialStock[material])
          player.setValue = player.materialStock[material];
        else
        {
          player.menu.titleLines = new List<string> {
          $"{player.setValue} de {material}. A quel prix unitaire ?",
          $"(Indiquez à l'oral le prix que vous souhaitez pour chaque unité de {material}"
          };

          Task playerInput = NwTask.Run(async () =>
          {
            int quantity = player.setValue;
            await NwTask.WaitUntil(() => player.oid.GetLocalVariable<int>("_PLAYER_INPUT").HasValue);
            if (player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value != Config.invalidInput)
              HandleRegisterMaterialToContract(player, material, quantity);
            else
              player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();
          });
        }
      }

      player.setValue = 0;
      player.menu.Draw();
    }
    private static void HandleRegisterMaterialToContract(PlayerSystem.Player player, string material, int quantity)
    {
      player.menu.Clear();
      uint availableGold = player.oid.Gold;

      if (player.setValue < 0)
      {
        player.menu.titleLines.Add($"Le prix indiqué n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => WriteContractPage(player)));
      }
      else
      {
        player.menu.titleLines.Add($"Contrat : {quantity} de {material} à {player.setValue} l'unité. Total : {quantity*player.setValue} pièces d'or.");
        player.menu.choices.Add(($"Ajouter une autre ressource au contrat.", () => WriteContractPage(player)));
        player.menu.choices.Add(($"Finaliser la rédaction du contrat.", () => CreateContractPage(player)));
      }

      player.setValue = 0;
      player.menu.Draw();
    }*/
  }
}
