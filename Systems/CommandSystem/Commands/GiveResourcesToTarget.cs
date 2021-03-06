﻿using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

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
      if (selection.IsCancelled || selection.TargetObject == null || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC)
        || !PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player targetPlayer))
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
        GetResourceQuantity(target, dm.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value);
        dm.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
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
        HandleGiveResources(target, material, int.Parse(dm.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value));
        dm.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
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
