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
      if (!PlayerSystem.Players.TryGetValue(selection.Player, out PlayerSystem.Player player))
        return;

      if (!(selection.TargetObject is NwPlayer) || ((NwPlayer)selection.TargetObject).IsDM)
      {
        selection.Player.SendServerMessage("La cible de l'écoute doit être un joueur.", Color.ORANGE);
        return;
      }

      NwPlayer oPC = (NwPlayer)selection.TargetObject;

      if (player.listened.Contains(oPC))
        player.listened.Remove(oPC);
      else
        player.listened.Add(oPC);
    }
  }
}
