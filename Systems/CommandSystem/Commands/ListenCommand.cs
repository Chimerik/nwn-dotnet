using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class ListenTarget
  {
    public ListenTarget(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionnner le joueur à écouter.", ColorConstants.Pink);
      oPC.EnterTargetMode(OnListenTargetSelected, ObjectTypes.Creature, MouseCursor.Magic);
    }
    private void OnListenTargetSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || !PlayerSystem.Players.TryGetValue(selection.Player.LoginCreature, out PlayerSystem.Player player))
        return;
      
      selection.Player.SendServerMessage("TEST.", ColorConstants.Orange);

      if (!(selection.TargetObject is NwCreature oPC) || oPC.ControllingPlayer.IsDM)
      {
        selection.Player.SendServerMessage("La cible de l'écoute doit être un joueur.", ColorConstants.Orange);
        return;
      }

      if (player.listened.Contains(oPC.ControllingPlayer))
      {
        player.listened.Remove(oPC.ControllingPlayer);
        selection.Player.SendServerMessage($"{oPC.ControllingPlayer.PlayerName.ColorString(ColorConstants.White)} vient d'être retiré de votre liste d'écoute.", ColorConstants.Rose);
      }
      else
      {
        player.listened.Add(oPC.ControllingPlayer);
        selection.Player.SendServerMessage($"{oPC.ControllingPlayer.PlayerName.ColorString(ColorConstants.White)} vient d'être ajouté à votre liste d'écoute.", ColorConstants.Rose);
      }
    }
  }
}
