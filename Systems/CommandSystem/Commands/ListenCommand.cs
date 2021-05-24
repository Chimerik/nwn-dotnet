using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class ListenTarget
  {
    public ListenTarget(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionnner le joueur à écouter.", Color.ROSE);
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, OnListenTargetSelected, ObjectTypes.Creature, MouseCursor.Magic);
    }
    private void OnListenTargetSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (!PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player))
        return;

      NwCreature oPC = (NwCreature)selection.TargetObject;

      if (oPC.ControllingPlayer != null || oPC.ControllingPlayer.IsDM)
      {
        selection.Player.SendServerMessage("La cible de l'écoute doit être un joueur.", Color.ORANGE);
        return;
      }

      if (player.listened.Contains(oPC.ControllingPlayer))
        player.listened.Remove(oPC.ControllingPlayer);
      else
        player.listened.Add(oPC.ControllingPlayer);
    }
  }
}
