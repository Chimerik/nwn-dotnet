using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class SetTargetTag
  {
    public SetTargetTag(NwPlayer oPC)
    {
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, ChangeTagTarget, ObjectTypes.All, MouseCursor.Create);
    }
    private static void ChangeTagTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (!PlayerSystem.Players.TryGetValue(selection.Player, out PlayerSystem.Player player) || selection.TargetObject == null)
        return;

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez saisir le nouveau tag."
        };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");

        player.oid.SendServerMessage($"{selection.TargetObject.Name.ColorString(Color.WHITE)} a été taggué {player.setString.ColorString(Color.WHITE)}.", Color.GREEN);
        player.setString = "";
        player.menu.Close();
      });

      player.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
  }
}
