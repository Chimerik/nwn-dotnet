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
      if (selection.TargetObject == null || !(selection.TargetObject is NwCreature oCreature)
        || !PlayerSystem.Players.TryGetValue(oCreature.LoginPlayer.LoginCreature, out PlayerSystem.Player targetPlayer))
        return;

      dm.menu.Clear();

      dm.menu.titleLines = new List<string>() {
        "Quelle ressource ?"
        };

      dm.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(dm)));
      dm.menu.choices.Add(("Quitter", () => dm.menu.Close()));
      dm.menu.Draw();

      WaitPlayerInput(targetPlayer);
    }
    private async void WaitPlayerInput(PlayerSystem.Player target)
    {
      bool awaitedValue = await dm.WaitForPlayerInputString();

      if (awaitedValue)
      {
        GetResourceQuantity(target, dm.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value);
        dm.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void GetResourceQuantity(PlayerSystem.Player target, string material)
    {
      dm.menu.Clear();

      dm.menu.titleLines = new List<string>() {
        "Quelle quantité ?"
        };

      dm.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(dm)));
      dm.menu.choices.Add(("Quitter", () => dm.menu.Close()));
      dm.menu.Draw();

      bool awaitedValue = await dm.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        HandleGiveResources(target, material, int.Parse(dm.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value));
        dm.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private void HandleGiveResources(PlayerSystem.Player player, string material, int quantity)
    {
      if (player.materialStock.ContainsKey(material))
        player.materialStock[material] += quantity;
      else
        player.materialStock.Add(material, quantity);

      dm.oid.SendServerMessage($"Vous venez de donner {quantity} unité(s) de {material} à {player.oid.LoginCreature.Name}");

      dm.menu.Close();
    }
  }
}
