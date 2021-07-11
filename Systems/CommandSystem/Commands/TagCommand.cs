using System.Collections.Generic;
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
    private static async void ChangeTagTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || !PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player) || selection.TargetObject == null)
        return;

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez saisir le nouveau tag."
        };

      player.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        selection.TargetObject.Tag = player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value;
        player.oid.SendServerMessage($"{selection.TargetObject.Name.ColorString(ColorConstants.White)} a été taggué {selection.TargetObject.Tag.ColorString(ColorConstants.White)}.", ColorConstants.Green);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
  }
}
