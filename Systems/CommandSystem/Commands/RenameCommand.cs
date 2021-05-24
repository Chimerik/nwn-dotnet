using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using System.Collections.Generic;

namespace NWN.Systems
{
  class DMRenameTarget
  {
    public DMRenameTarget(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionner la cible à renommer");
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, RenameTarget, ObjectTypes.All, MouseCursor.Create);
    }
    private async void RenameTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (!PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player) || selection.TargetObject == null)
        return;

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez saisir le nouveau nom."
        };

      //player.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        selection.TargetObject.Name = player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"{selection.TargetObject.Name.ColorString(Color.WHITE)} a été renommé {selection.TargetObject.Name.ColorString(Color.WHITE)}.", Color.GREEN);
        player.menu.Close();
      }
    }
  }
}
