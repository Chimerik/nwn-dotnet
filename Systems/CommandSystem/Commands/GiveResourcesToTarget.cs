using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class GiveResourcesToTarget
  {
    PlayerSystem.Player dm;
    public GiveResourcesToTarget(PlayerSystem.Player player)
    {
      dm = player;
      dm.oid.SendServerMessage("Veuillez sélectionner la cible du don.");
      PlayerSystem.cursorTargetService.EnterTargetMode(dm.oid, SelectTarget, ObjectTypes.Creature, MouseCursor.Create);
    }
    private void SelectTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.TargetObject == null || !(selection.TargetObject is NwPlayer)
        || !PlayerSystem.Players.TryGetValue(selection.TargetObject, out PlayerSystem.Player targetPlayer))
        return;

      dm.menu.Clear();

      dm.menu.titleLines = new List<string>() {
        "Quelle ressource ?"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        dm.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        dm.setString = "";
        await NwTask.WaitUntil(() => dm.setString != "");

        GetResourceQuantity(targetPlayer, dm.setString);
        dm.setString = "";
      });

      dm.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(dm)));
      dm.menu.choices.Add(("Quitter", () => dm.menu.Close()));
      dm.menu.Draw();
    }
    private void GetResourceQuantity(PlayerSystem.Player target, string material)
    {
      dm.menu.Clear();

      dm.menu.titleLines = new List<string>() {
        "Quelle quantité ?"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        dm.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        dm.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => dm.setValue != Config.invalidInput);

        HandleGiveResources(target, material, dm.setValue);
        dm.setValue = Config.invalidInput;
      });

      dm.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(dm)));
      dm.menu.choices.Add(("Quitter", () => dm.menu.Close()));
      dm.menu.Draw();
    }
    private void HandleGiveResources(PlayerSystem.Player player, string material, int quantity)
    {
      if (player.materialStock.ContainsKey(material))
        player.materialStock[material] += quantity;
      else
        player.materialStock.Add(material, quantity);

      dm.oid.SendServerMessage($"Vous venez de donner {quantity} unité(s) de {material} à {player.oid.Name}");

      dm.menu.Close();
    }
  }
}
