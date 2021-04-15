using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using System.Threading.Tasks;
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
    private void RenameTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (!PlayerSystem.Players.TryGetValue(selection.Player, out PlayerSystem.Player player) || selection.TargetObject == null)
        return;

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez saisir le nouveau nom."
        };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");

        player.oid.SendServerMessage($"{selection.TargetObject.Name.ColorString(Color.WHITE)} a été renommé {player.setString.ColorString(Color.WHITE)}.", Color.GREEN);
        player.setString = "";
        player.menu.Close();
      });

      player.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
